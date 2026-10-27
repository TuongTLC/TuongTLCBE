using StackExchange.Redis;
using System.Text.Json;

namespace TuongTLCBE.Business.CacheService;

public class CacheService : ICacheService, IDisposable
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly IServer _server;

    public CacheService(IConfiguration configuration)
    {
        string redisConnection = configuration["Redis:ConnectionString"] 
            ?? Environment.GetEnvironmentVariable("RedisConnectionString") 
            ?? "localhost:6379,password=itslocalhost";

        if (string.IsNullOrWhiteSpace(redisConnection))
        {
            redisConnection = "localhost:6379,password=itslocalhost";
        }

        // abortConnect=false so a temporarily unavailable Redis won't crash startup;
        // StackExchange.Redis will keep retrying in the background.
        ConfigurationOptions options = ConfigurationOptions.Parse(redisConnection);
        options.AbortOnConnectFail = false;
        _redis = ConnectionMultiplexer.Connect(options);

        _database = _redis.GetDatabase();
        // Resolve the server from the actual connected endpoints instead of
        // re-parsing the (possibly password-bearing) connection string.
        _server = _redis.GetServer(_redis.GetEndPoints().First());
    }

    public T? GetData<T>(string key)
    {
        RedisValue value = _database.StringGet(key);
        if (value.IsNullOrEmpty)
        {
            return default;
        }
        try
        {
            return JsonSerializer.Deserialize<T>(value.ToString());
        }
        catch
        {
            return default;
        }
    }

    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
        TimeSpan expireTime = expirationTime.UtcDateTime.Subtract(DateTime.UtcNow);
        if (expireTime < TimeSpan.Zero)
        {
            expireTime = TimeSpan.FromMinutes(1);
        }
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

    public void Dispose()
    {
        _redis.Dispose();
    }
}