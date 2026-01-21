using Microsoft.AspNetCore.Mvc;

namespace Web.API.Middleware
{
    public sealed class GlobalExceptionsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionsMiddleware> _logger;

        public GlobalExceptionsMiddleware(RequestDelegate next, ILogger<GlobalExceptionsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Occured an error: {ex}");

                context.Response.StatusCode = ex switch
                {
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    InvalidOperationException or ArgumentException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };

                await context.Response.WriteAsJsonAsync(
                    new ProblemDetails
                    {
                        Type = ex.GetType().Name,
                        Detail = "An error occured",
                        Status = context.Response.StatusCode
                    });
            }
        }
    }

    public static class GlobalExceptionsMiddlewareExtension
    {
        public static IApplicationBuilder UseGlobalExceptions(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionsMiddleware>();
        }
    }
}
