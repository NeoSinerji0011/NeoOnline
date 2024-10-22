using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;


namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public virtual int Count
        {
            get { return _dbSet.Count(); }
        }

        public virtual IQueryable<T> All()
        {
            return _dbSet.AsQueryable();
        }

        public virtual T FindById(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual T FindById(object[] keyValues)
        {
            return _dbSet.Find(keyValues);
        }

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!String.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                return orderBy(query).AsQueryable();
            }
            else
            {
                return query.AsQueryable();
            }
        }

        public virtual IQueryable<T> Filter(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).AsQueryable();
        }

        public virtual IQueryable<T> Filter(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50)
        {
            int skipCount = index * size;
            var resetSet = filter != null ? _dbSet.Where(filter).AsQueryable() : _dbSet.AsQueryable();
            resetSet = skipCount == 0 ? resetSet.Take(size) : resetSet.Skip(skipCount).Take(size);
            total = resetSet.Count();
            return resetSet.AsQueryable();
        }

        public virtual IQueryable<T> Page(IQueryable<T> query, Expression<Func<T, object>> orderBy, bool isAscending, int page, int pageSize, out int rowCount)
        {
            if (pageSize <= 0) pageSize = 10;

            rowCount = query.Count();

            if (rowCount <= pageSize || page <= 0) page = 1;

            int excludedRows = (page - 1) * pageSize;

            query = this.OrderBy(query, orderBy, isAscending);

            return query.Skip(excludedRows).Take(pageSize);
        }

        private IQueryable<T> OrderBy(IQueryable<T> query, Expression<Func<T, object>> orderBy, bool isAscending)
        {
            var unaryExpression = orderBy.Body as UnaryExpression;
            if (unaryExpression != null)
            {
                var propertyExpression = (MemberExpression)unaryExpression.Operand;
                var parameters = orderBy.Parameters;

                if (propertyExpression.Type == typeof(DateTime))
                {
                    var newExpression = Expression.Lambda<Func<T, DateTime>>(propertyExpression, parameters);
                    return isAscending ? query.OrderBy(newExpression) : query.OrderByDescending(newExpression);
                }
                if (propertyExpression.Type == typeof(int))
                {
                    var newExpression = Expression.Lambda<Func<T, int>>(propertyExpression, parameters);
                    return isAscending ? query.OrderBy(newExpression) : query.OrderByDescending(newExpression);
                }
                if (propertyExpression.Type == typeof(short))
                {
                    var newExpression = Expression.Lambda<Func<T, short>>(propertyExpression, parameters);
                    return isAscending ? query.OrderBy(newExpression) : query.OrderByDescending(newExpression);
                }
                if (propertyExpression.Type == typeof(string))
                {
                    var newExpression = Expression.Lambda<Func<T, string>>(propertyExpression, parameters);
                    return isAscending ? query.OrderBy(newExpression) : query.OrderByDescending(newExpression);
                }

                throw new NotSupportedException("Object type resolution not implemented for this type");
            }
            else if (orderBy.NodeType == ExpressionType.MemberAccess || orderBy.NodeType == ExpressionType.Lambda)
            {
                var propertyExpression = orderBy.Body as MemberExpression;
                var parameters = orderBy.Parameters;

                if (propertyExpression != null)
                {
                    if (propertyExpression.Type == typeof(string))
                    {
                        var newExpression = Expression.Lambda<Func<T, string>>(propertyExpression, parameters);
                        return isAscending ? query.OrderBy(newExpression) : query.OrderByDescending(newExpression);
                    }
                }
            }

            return query.OrderBy(orderBy);
        }

        public virtual T Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public virtual T Create(T entity)
        {
            var newEntry = _dbSet.Add(entity);
            return newEntry;
        }

        public virtual void Delete(object id)
        {
            var entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(T entity)
        {
            if (_dbContext.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }

        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            var entitiesToDelete = Filter(predicate);
            foreach (var entity in entitiesToDelete)
            {
                if (_dbContext.Entry(entity).State == EntityState.Detached)
                {
                    _dbSet.Attach(entity);
                }
                _dbSet.Remove(entity);
            }
        }

        public virtual void Update(T entity)
        {
            var entry = _dbContext.Entry(entity);
            _dbSet.Attach(entity);
            entry.State = EntityState.Modified;
        }


    }
}
