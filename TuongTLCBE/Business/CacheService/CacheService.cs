using System.Text.Json;
using StackExchange.Redis;

namespace TuongTLCBE.Business.CacheService;

public class CacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly IServer _server;

    public CacheService()
    {
        var redis = ConnectionMultiplexer.Connect("192.168.1.32:6379, password=itslocalhost, allowAdmin=true");
        _database = redis.GetDatabase();
        _server = redis.GetServer("192.168.1.32:6379");
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

    public object RemoveData(string key)
    {
        var exist = _database.KeyExists(key);
        if (exist) return _database.KeyDelete(key);

        return false;
    }
    public void FlushData()
    {
        _server.FlushDatabase();
    }
}