namespace TuongTLCBE.Business.CacheService;

public interface ICacheService
{
    public T? GetData<T>(string key);
    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime);
    public Task<object> RemoveData(string key);
    public void FlushData();
    public Task<object> RemoveOldCache(string prefix);
}