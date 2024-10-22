using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        ///   Get the total objects count.
        /// </summary>
        int Count { get; }

        /// <summary>
        ///   Gets all objects from database
        /// </summary>
        IQueryable<T> All();

        /// <summary>
        ///   Gets object by primary key.
        /// </summary>
        /// <param name="id"> primary key </param>
        /// <returns> </returns>
        T FindById(object id);

        /// <summary>
        ///   Gets object by primary keys.
        /// </summary>
        /// <param name="id"> primary key </param>
        /// <returns> </returns>
        T FindById(object[] keyValues);

        /// <summary>
        ///   Gets objects via optional filter, sort order, and includes
        /// </summary>
        /// <param name="filter"> </param>
        /// <param name="orderBy"> </param>
        /// <param name="includeProperties"> </param>
        /// <returns> </returns>
        IQueryable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");

        /// <summary>
        ///   Gets objects from database by filter.
        /// </summary>
        /// <param name="predicate"> Specified a filter </param>
        IQueryable<T> Filter(Expression<Func<T, bool>> predicate);

        /// <summary>
        ///   Gets objects from database with filtering and paging.
        /// </summary>
        /// <param name="filter"> Specified a filter </param>
        /// <param name="total"> Returns the total records count of the filter. </param>
        /// <param name="index"> Specified the page index. </param>
        /// <param name="size"> Specified the page size </param>
        IQueryable<T> Filter(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50);

        IQueryable<T> Page(IQueryable<T> query, Expression<Func<T, object>> orderBy, bool isAscending, int page, int pageSize, out int rowCount);


        /// <summary>
        ///   Find object by specified expression.
        /// </summary>
        /// <param name="predicate"> </param>
        T Find(Expression<Func<T, bool>> predicate);

        /// <summary>
        ///   Create a new object to database.
        /// </summary>
        /// <param name="entity"> Specified a new object to create. </param>
        T Create(T entity);

        /// <summary>
        ///   Deletes the object by primary key
        /// </summary>
        /// <param name="id"> </param>
        void Delete(object id);

        /// <summary>
        ///   Delete the object from database.
        /// </summary>
        /// <param name="entity"> Specified a existing object to delete. </param>
        void Delete(T entity);

        /// <summary>
        ///   Update object changes and save to database.
        /// </summary>
        /// <param name="entity"> Specified the object to save. </param>
        void Update(T entity);
    }
}
