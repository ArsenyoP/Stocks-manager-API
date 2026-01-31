using System.Diagnostics;

namespace Web.API.Middleware
{
    public sealed class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimingMiddleware> _logger;
        public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            await _next(context);
            stopwatch.Stop();

            var requestTime = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation("Запит {@Method} {@Path} виконано за {@Elapsed} мс",
                context.Request.Method,
                context.Request.Path.Value,
                requestTime);
        }
    }

    public static class RequestTimingMiddlewareExtension
    {
        public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestTimingMiddleware>();
        }
    }
}
