using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AHAApi
{
    public interface IBaseRepository<T>
    {
        Task<T> GetByIdAsync(string id);
        Task<List<T>> GetAllAsync();
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id);
    }
}
