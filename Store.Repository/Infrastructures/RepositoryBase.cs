using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KStore.Models;
using Microsoft.EntityFrameworkCore;
using Store.Repositories.Interfaces;
using Store.Services.EF;


namespace Store.Repositories.Infrastructure
{
    public class RepositoryBase<T> : IRepository<T> where T : class, IBaseEntity
    {
        protected StoreContext _context;

        public RepositoryBase(StoreContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Method add entity to db
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        public async Task Add(T obj, bool commit = true)
        {
            // add object to entity
            _context.Set<T>().Add(obj);

            // if commit save to db
            if (commit)
                await Commit();
        }

        /// <summary>
        /// Method using save changes to database
        /// </summary>
        /// <returns></returns>
        public async Task Commit()
        {
            // save changes to db
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Method using delete object 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="commit"></param>
        /// <returns>List<Object></returns>
        public async Task Delete(T obj, bool commit = true)
        {
            // remove object 
            _context.Set<T>().Remove(obj);

            // if commit save to db
            if (commit)
                await Commit();
        }


        /// <summary>
        /// Method using find, orderby object by conditions
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <returns>IEnumerable<T></returns>
        public T FirstOrDefault(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "")
        {
            // get object from database
            IQueryable<T> query = _context.Set<T>();
            query = query.Where(i => !i.IsDeleted);

            // if fillter
            if (filter != null)
            {
                // fillter by condition
                query = query.Where(filter);
            }

            // get list properties to display
            var properties = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // loop properties
            foreach (var includeProperty in properties)
            {
                // Get the desired properties of the object
                query = query.Include(includeProperty);
            }

            // if order by
            if (orderBy != null)
            {
                // sort query
                query = orderBy(query);
            }

            // return result
            return query.FirstOrDefault();
        }

        public bool Any(Expression<Func<T, bool>>? filter = null)
        {
            // get object from database only query data
            IQueryable<T> query = _context.Set<T>().AsNoTracking();


            // if fillter
            if (filter != null)
            {
                // fillter by condition
                query = query.Where(filter);
            }

            return query.Any();
        }

        public async Task<int> Count(Expression<Func<T, bool>>? filter = null)
        {
            // get object from database only query data
            IQueryable<T> query = _context.Set<T>().AsNoTracking();


            // if fillter
            if (filter != null)
            {
                // fillter by condition
                query = query.Where(filter);
            }

            return await query.CountAsync();
        }

        /// <summary>
        /// method using get data from table 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <returns>IQueryable<Object></returns>
        public async Task<ICollection<T>> QueryAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "", int pageSize = 0, int page = -1)
        {
            // get object from database only query data
            IQueryable<T> query = _context.Set<T>()
                .Where(i => !i.IsDeleted)
                .AsNoTracking();


            // if fillter
            if (filter != null)
            {
                // fillter by condition
                query = query.Where(filter);
            }

            // get list properties to display
            var properties = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // loop properties
            foreach (var includeProperty in properties)
            {
                // Get the desired properties of the object
                query = query.Include(includeProperty);
            }


            // if order by
            if (orderBy != null)
            {
                // sort query
                query = orderBy(query);
            }

            //if paging
            if (pageSize > 0 && page > -1)
            {
                query = query.Skip(pageSize * page).Take(pageSize);
            }

            // return result
            return await query.ToListAsync();
        }

        public async Task<ICollection<TResult>> QueryAndSelectAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                string includeProperties = "", int pageSize = 0, int page = -1) where TResult : class
        {
            // get object from database only query data
            IQueryable<T> query = _context.Set<T>()
                .Where(i => !i.IsDeleted)
                .AsNoTracking();

            // if filter
            if (filter != null)
            {
                // fillter by condition
                query = query.Where(filter);
            }

            // get list properties to display
            var properties = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // loop properties
            foreach (var includeProperty in properties)
            {
                // Get the desired properties of the object
                query = query.Include(includeProperty);
            }


            // if order by
            if (orderBy != null)
            {
                // sort query
                query = orderBy(query);
            }

            //if paging
            if (pageSize > 0 && page > -1)
            {
                query = query.Skip(pageSize * page).Take(pageSize);
            }
            else
            {
                if (pageSize > 0)
                {
                    query = query.Take(pageSize);
                }
            }


            //project and return

            // return result
            return await query.Select(selector).ToListAsync();
        }

        /// <summary>
        /// Get Object by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeProperties"></param>
        /// <returns>Object</returns>
        public async Task<T> Get(int id, string includeProperties = "")
        {
            //
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return await query.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);
        }

        public async Task<T> Get(Expression<Func<T, bool>> filter, string includeProperties = "")
        {
            IQueryable<T> query = _context.Set<T>();
            query = query.Where(i => !i.IsDeleted);

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task<List<T>> Get(string? includeProperties = "")
        {
            if (string.IsNullOrEmpty(includeProperties))
                return await _context.Set<T>().Where(i => !i.IsDeleted).ToListAsync();
            IQueryable<T> query = _context.Set<T>();
            query = query.Where(i => !i.IsDeleted);

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return await query.ToListAsync();
        }

        public async Task Update(T obj, bool commit = true)
        {
            _context.Set<T>().Update(obj);
            if (commit)
                await _context.SaveChangesAsync();
        }

        public async Task TruncateTable(string tableName)
        {
            string cmd = $"TRUNCATE TABLE {tableName}";
            await _context.Database.ExecuteSqlRawAsync(cmd);
        }
    }

}
