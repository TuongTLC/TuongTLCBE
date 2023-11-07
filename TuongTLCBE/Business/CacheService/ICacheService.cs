namespace TuongTLCBE.Business.CacheService;

public interface ICacheService
{
    string? GetData(string key);
    bool SetData<T>(string key, T value, DateTimeOffset expirationTime);
    object RemoveData(string key);
}