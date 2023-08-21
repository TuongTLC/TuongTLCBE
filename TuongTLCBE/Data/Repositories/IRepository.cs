namespace TuongTLCBE.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> Get(Guid id);
        Task<T?> Insert(T entity);
        Task Update();
    }
}

