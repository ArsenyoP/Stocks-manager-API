using StackExchange.Redis;
using Web.API.Interfaces.IServices;

namespace Web.API.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
        {
            _redis = redis;
            _logger = logger;
        }

        public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: $"{prefix}*").ToArray();

            if (keys.Length == 0)
            {
                return;
            }

            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(keys);

            _logger.LogInformation("Removed {Count} keys with prefix {Prefix}", keys.Length, prefix);
        }
    }
}
