using System.Text.Json;
using StackExchange.Redis;

namespace TuongTLCBE.Business.CacheService;

public class CacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly IServer _server;

    
    public CacheService()
    {
        var redis = ConnectionMultiplexer.Connect("localhost:6379, password=itslocalhost, allowAdmin=true");
        _database = redis.GetDatabase();
        _server = redis.GetServer("localhost:6379");
    }

    public T? GetData<T>(string key)
    {
        var value = _database.StringGet(key);
        if (!string.IsNullOrWhiteSpace(value)) return JsonSerializer.Deserialize<T>(value);
        return default;
    }

    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
        var expireTime = expirationTime.DateTime.Subtract(DateTime.Now);
        var isSet = _database.StringSet(key, JsonSerializer.Serialize(value), expireTime);
        return isSet;
    }

    public async Task<object> RemoveData(string key)
    {
        try
        {
            var exist = await _database.KeyExistsAsync(key);
            if (exist) return await _database.KeyDeleteAsync(key);
            return false;
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
            var keys = _server.Keys(pattern: prefix + "*").ToList();
            if (!keys.Any()) return "No matching keys!";
            foreach (var key in keys) await RemoveData(key);
            return true;
        }
        catch (Exception e)
        {
            return e;
        }
    }
}