using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web.API.Models;

namespace Web.API.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions) :
            base(dbContextOptions)
        {

        }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Portfolio>(x => x.HasKey(p => new { p.AppUserId, p.StockId }));

            builder.Entity<Portfolio>()
                .HasOne(u => u.AppUser)
                .WithMany(u => u.Portfolios)
                .HasForeignKey(p => p.AppUserId);

            builder.Entity<Portfolio>()
                .HasOne(u => u.Stock)
                .WithMany(u => u.Portfolios)
                .HasForeignKey(p => p.StockId);

            builder.Entity<Comment>()
                .HasOne(c => c.Stock)
                .WithMany(s => s.Comments)
                .HasForeignKey(c => c.StockID)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<Comment>()
                .HasIndex(c => c.CreatedOn)
                .IsDescending(true)
                .IncludeProperties(c => new { c.Title, c.Content, c.AppUserId })
                .HasDatabaseName("IX_Comments_CreatedOn_Covering");

            builder.Entity<Comment>()
                .HasIndex(c => new { c.StockID, c.CreatedOn })
                .IsDescending(false, true)
                .IncludeProperties(c => new { c.Title, c.Content, c.AppUserId })
                .HasDatabaseName("IX_Comments_StockID_CreatedOn_Covering");






            builder.Entity<Portfolio>()
                .HasIndex(c => new { c.AppUserId })
                .HasDatabaseName("IX_Portfolio_AppUserId");

            builder.Entity<Stock>()
                .HasIndex(s => s.Symbol)
                .IsUnique();

            builder.Entity<Stock>()
                .HasIndex(s => s.CompanyName);

            builder.Entity<Stock>()
                .HasIndex(s => new { s.LastDiv, s.MarketCap })
                .HasDatabaseName("IX_Stocks_LastDiv_MarketCap");


            const string adminRoleId = "3474648c-112d-460c-bc8a-61be046b3078";
            const string userRoleId = "eb4b3260-05fb-470c-af9f-b9e9b9bcd85e";

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = userRoleId,
                    Name = "User",
                    NormalizedName = "USER"
                },
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);

            const string adminId = "f5b7c8d9-e0a1-4b2c-3d4e-5f6a7b8c9d0e";

            var admin = new AppUser
            {
                Id = adminId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "arsenyo198510@gmail.com",
                NormalizedEmail = "ARSENYO198510@GMAIL.COM",
                EmailConfirmed = true,
                SecurityStamp = "a1b2c3d4-e5f6-7890-abcd-000000000000",
                ConcurrencyStamp = "b2c3d4e5-f6a7-8901-bcde-111111111111"
            };

            admin.PasswordHash = new PasswordHasher<AppUser>().HashPassword(admin, "1234");

            builder.Entity<AppUser>().HasData(admin);

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminId
            });

        }
    }
}
