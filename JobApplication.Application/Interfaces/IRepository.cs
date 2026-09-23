using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Interfaces
{
    public interface IRepository<T>
    {
        Task AddAsync(T entity);
        void Update(T entity);
        IQueryable<T> Get();
        void Remove(T entity);
        Task SaveChangesAsync();
    }
}
