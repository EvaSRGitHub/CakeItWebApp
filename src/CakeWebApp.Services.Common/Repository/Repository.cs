using CakeItWebApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CakeItWebApp.Services.Common.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity:class
    {
        private readonly CakeItDbContext db;
        private DbSet<TEntity> dbSet;

        public Repository(CakeItDbContext db)
        {
            this.db = db;
            this.dbSet = db.Set<TEntity>();
        }

        public void Add(TEntity entity)
        {
            this.dbSet.Add(entity);
        }

        public IQueryable<TEntity> All()
        {
            return this.dbSet;
        }

        public void Delete(TEntity entity)
        {
            this.dbSet.Remove(entity);
        }

        public Task<TEntity> GetByIdAsync(params object[] id)
        {
            return this.dbSet.FindAsync(id);
        }

        public Task<int> SaveChangesAsync()
        {
           return this.db.SaveChangesAsync();
        }

        public void Update(TEntity entity)
        {
            var entry = this.db.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                this.dbSet.Attach(entity);
            }

            entry.State = EntityState.Modified;
        }

        public void Dispose() => this.db.Dispose();

        public void DeleteRange(IQueryable<TEntity> entities)
        {
            foreach (var item in entities)
            {
                this.Delete(item);
            }
        }
    }
}
