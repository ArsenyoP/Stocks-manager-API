using Hangfire;
using Web.API.Jobs;

namespace Web.API.Extensions
{
    public static class HangfireExtensions
    {
        public static IApplicationBuilder UseHangfireJobs(
            this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var recurringJobManager = scope.ServiceProvider
                .GetRequiredService<IRecurringJobManager>();

            recurringJobManager.AddOrUpdate<CacheCleanupJob>(
                "cache-cleanup",
                job => job.ExecuteAsync(),
                Cron.MinuteInterval(10));

            return app;
        }
    }
}
