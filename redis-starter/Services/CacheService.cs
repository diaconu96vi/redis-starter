using StackExchange.Redis;
using System.Text.Json;

namespace Redis.Starter.Services
{
    public class CacheService : ICacheService
    {
        private readonly ILogger<CacheService> _logger;
        private readonly IDatabase _cacheDb;

        public CacheService(ILogger<CacheService> logger, IConnectionMultiplexer connectionMultiplexer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cacheDb = connectionMultiplexer?.GetDatabase() ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        }
        public async Task<RedisValue> GetData<T>(string key)
        {
            ArgumentNullException.ThrowIfNull(key);
            if (!await _cacheDb.KeyExistsAsync(key))
            {
                return default;
            }

            return await _cacheDb.StringGetAsync(key);
        }

        public async Task<bool> SetData(string key, object data)
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(data);

            if (await _cacheDb.KeyExistsAsync(key))
            {
                return true;
            }

            return await _cacheDb.StringSetAsync(key, JsonSerializer.Serialize(data));
        }
    }
}
