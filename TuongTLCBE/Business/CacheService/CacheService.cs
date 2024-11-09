using StackExchange.Redis;
using System.Text.Json;

namespace TuongTLCBE.Business.CacheService;

public class CacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly IServer _server;


    public CacheService()
    {
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379, password=itslocalhost, allowAdmin=true");
        _database = redis.GetDatabase();
        _server = redis.GetServer("localhost:6379");
    }

    public T? GetData<T>(string key)
    {
        RedisValue value = _database.StringGet(key);
        return !string.IsNullOrWhiteSpace(value) ? JsonSerializer.Deserialize<T>(value) : default;
    }

    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
        TimeSpan expireTime = expirationTime.DateTime.Subtract(DateTime.Now);
        bool isSet = _database.StringSet(key, JsonSerializer.Serialize(value), expireTime);
        return isSet;
    }

    public async Task<object> RemoveData(string key)
    {
        try
        {
            bool exist = await _database.KeyExistsAsync(key);
            return exist ? await _database.KeyDeleteAsync(key) : (object)false;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public void FlushData()
    {
        _server.FlushDatabase();
    }

    public async Task<object> RemoveOldCache(string prefix)
    {
        try
        {
            List<RedisKey> keys = _server.Keys(pattern: prefix + "*").ToList();
            if (!keys.Any())
            {
                return "No matching keys!";
            }

            foreach (RedisKey key in keys)
            {
                _ = await RemoveData(key);
            }

            return true;
        }
        catch (Exception e)
        {
            return e;
        }
    }
}