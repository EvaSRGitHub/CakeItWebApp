﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeItWebApp.Data.Repository
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
    }
}
