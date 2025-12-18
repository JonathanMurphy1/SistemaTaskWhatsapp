using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string? includeProperties = null
            );

        IQueryable<T> GetAllQueryable(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null);

        Task<T> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> filter = null,
            string? includeProperties = null
            );

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        Task<bool> RemoveByIdAsync(int id);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);

        void Update(T entity);

    }
}
