﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeItWebApp.Services.Common.Repository
{
    public interface IRepository<TEntity> where TEntity: class
    {
        IQueryable<TEntity> All();

        Task<TEntity> GetByIdAsync(params object[] id);

        void Add(TEntity entity);

        void Delete(TEntity entity);

        Task<int> SaveChangesAsync();

        void Update(TEntity entity);

        void Dispose();
    }
}