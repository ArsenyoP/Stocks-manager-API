
using Web.API.Interfaces.IServices;

namespace Web.API.Services.Background_Services
{
    public class CacheCleanupBackgroundJob : BackgroundService
    {
        private readonly ILogger<CacheCleanupBackgroundJob> _logger;
        private readonly IRedisCacheService _redisService;

        public CacheCleanupBackgroundJob(ILogger<CacheCleanupBackgroundJob> logger, IRedisCacheService redisServer)
        {
            _logger = logger;
            _redisService = redisServer;
        }


        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await _redisService.RemoveByPrefixAsync("stock-", ct);
                    _logger.LogInformation("Cache has been successfully cleaned");
                    Console.WriteLine($"Cache has been successfully cleaned {DateTime.Now}");
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Background service has been stopped");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Cache cleanup failed");
                }

                await Task.Delay(TimeSpan.FromMinutes(10));
            }
        }
    }
}
