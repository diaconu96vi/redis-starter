using StackExchange.Redis;

namespace Redis.Starter.Services
{
    public interface ICacheService
    {
        Task<RedisValue> GetData<T>(string key);

        Task<bool> SetData(string key, object data);
    }
}
