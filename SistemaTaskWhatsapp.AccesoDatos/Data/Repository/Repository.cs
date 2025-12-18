using Microsoft.EntityFrameworkCore;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.AccesoDatos.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext Context;
        internal DbSet<T> dbSet;

        public Repository(DbContext context)
        {
            Context = context;
            this.dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string? includeProperties = null)
        {
            //Se crea una consulta IQueryable a partir del DbSet del contexto
            IQueryable<T> query = dbSet;

            if(filter != null)
            {
                query = query.Where(filter);
            }

            //Se incluyen propiedades de navegacion si se proporcionan
            if(includeProperties != null)
            {
                foreach(var includeProperty in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            //Se aplica el ordenamiento si se proporciona
            if(orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public IQueryable<T> GetAllQueryable(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            //Aplicar filtro si se proporciona
            if (filter != null)
            {
                query = query.Where(filter);
            }

            //Incluir propiedades de navegación si se especifican
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(
                    new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            //no ejecutamos la consulta todavía
            return query;
        }

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null, string? includeProperties = null)
        {
            //Se crea una consulta IQueryable a partir del DbSet del contexto
            IQueryable<T> query = dbSet;

            //Se aplica filtro si se proporciona
            if (filter != null)
            {
                query = query.Where(filter);
            }

            //Se incluyen propiedades de navegacion si se proporcionan
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<bool> RemoveByIdAsync(int id) 
        { 
            try 
            { 
                T entityToRemove = await dbSet.FindAsync(id); 

                if (entityToRemove == null) 
                    return false; 

                dbSet.Remove(entityToRemove); 
                
                return true; 
            } 
            catch (Exception) 
            { 
                return false; 
            } 
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await dbSet.AddRangeAsync(entities);
        }
    }
}
