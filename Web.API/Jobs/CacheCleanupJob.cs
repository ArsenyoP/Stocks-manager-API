using Web.API.Interfaces.IServices;

namespace Web.API.Jobs
{
    public class CacheCleanupJob
    {
        private readonly ILogger<CacheCleanupJob> _logger;
        private readonly IRedisCacheService _redis;

        public CacheCleanupJob(ILogger<CacheCleanupJob> logger,
            IRedisCacheService redis)
        {
            _logger = logger;
            _redis = redis;
        }

        public async Task ExecuteAsync()
        {
            await _redis.RemoveByPrefixAsync("stock-");
            _logger.LogInformation("CacheCleanupJob job has been executed");
        }

    }
}
