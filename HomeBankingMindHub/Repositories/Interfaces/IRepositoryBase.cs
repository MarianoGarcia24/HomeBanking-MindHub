﻿using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindAll(Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> condition);

        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        void SaveChanges();

    }
}
