using Microsoft.EntityFrameworkCore;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.Data.Repositories
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        protected readonly TuongTlcdbContext context;
        private readonly DbSet<T> _entities;

        public Repository(TuongTlcdbContext context)
        {
            this.context = context;
            _entities = context.Set<T>();
        }

        public async Task<T?> Get(Guid id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task<T?> Insert(T entity)
        {
            _ = await _entities.AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;
        }
        public async Task<int> Delete(T entity)
        {
            _ = _entities.Remove(entity);
            return await context.SaveChangesAsync();

        }
    }
}
