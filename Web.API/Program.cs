using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Web.API.Data;
using Web.API.Interfaces;
using Web.API.Interfaces.IServices;
using Web.API.Middleware;
using Web.API.Models;
using Web.API.Repository;
using Web.API.Services;
using Scrutor;
using Web.API.Services.Decorators;
using StackExchange.Redis;
using Hangfire;
using Web.API.Extensions;
using Web.API.Models.Settings;
using Web.API.Helpers;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace Web.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Services Configuration
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);
            builder.Services.AddFluentValidationAutoValidation();

            //setting up logging
            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));

            builder.Services.AddHttpClient<IFinancialService, FinancialService>();


            builder.Services.AddStackExchangeRedisCache(options =>
                options.Configuration = builder.Configuration.GetConnectionString("Redis"));

            builder.Services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            builder.Services.AddDbContext<ApplicationDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

                options.ConfigureWarnings(warnings =>
                    warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            });

            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 4;
            })
            .AddEntityFrameworkStores<ApplicationDBContext>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                options.DefaultChallengeScheme =
                options.DefaultForbidScheme =
                options.DefaultScheme =
                options.DefaultSignInScheme =
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
                    )
                };
            });

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
            });


            var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
            var hangfireConnection = System.Text.RegularExpressions.Regex
                .Replace(defaultConnection, @"Database=[^;]+;", "Database=master;");

            builder.Services.AddHangfire(config =>
            {
                config.UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(hangfireConnection);
            });

            builder.Services.AddHangfireServer();



            // Dependency injection
            builder.Services.AddScoped<IStockRepository, StockRepository>();
            builder.Services.AddScoped<ICommentsRepository, CommentRepository>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IPorrfolioRepository, PortfolioRepository>();

            builder.Services.AddScoped<ITokenService, TokenService>();

            builder.Services.AddScoped<IStockService, StockService>();
            builder.Services.Decorate<IStockService, CachedStockService>();

            builder.Services.AddScoped<IPorfolioService, PortfolioService>();

            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.Decorate<IAccountService, EmailNotificationAccountService>();

            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IFinancialService, FinancialService>();

            builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<IEmailService, EmailService>();

            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy("fixed", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromSeconds(20),
                            PermitLimit = 20,
                            QueueLimit = 0
                        }));


                options.AddPolicy("auth-limiter", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"auth_{context.Connection.RemoteIpAddress?.ToString() ?? "unknown"}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromSeconds(20),
                            PermitLimit = 5,
                            QueueLimit = 0
                        }));

            });



            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var retries = 10;
                while (retries > 0)
                {
                    try
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                        context.Database.Migrate();
                        Console.WriteLine("Database is ready and migrated.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        retries--;
                        Console.WriteLine($"Waiting for DB... Retries left: {retries}. Error: {ex.Message}");
                        await Task.Delay(5000);
                        if (retries == 0) throw;
                    }
                }
            }


            app.UseGlobalExceptions();
            app.UseRequestTiming();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRateLimiter();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new AllowAllDashboardAuthorizationFilter() }
            });

            app.UseHangfireJobs();
            app.MapControllers();

            app.Run();
        }
    }
}