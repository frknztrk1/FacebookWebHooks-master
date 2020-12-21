using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FacebookWebHooks
{
    public class GeneralRepository<T> : IRepository<T>
            where T : class


    {
        readonly AppDbContext _context;
        public GeneralRepository(AppDbContext context)
        {
            _context = context;
        }
        protected const string DefaultPrimaryKey = "Id";

        public int Add(T entity)
        {

            _context.BeginTransaction();
            try
            {
                _context.Set<T>().Add(entity);
                _context.Commit();
                return Convert.ToInt32(GetPropertyValue(entity, DefaultPrimaryKey));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _context.Rollback();
                return 0;
            }
        }
        public bool AddRange(IEnumerable<T> collection)
        {
            _context.BeginTransaction();

            try
            {
                _context.Set<T>().AddRange(collection);
                _context.Commit();
                return true;
            }
            catch (Exception)
            {
                _context.Dispose();
                return false;
            }
        }
        public bool Delete(int id)
        {
            _context.BeginTransaction();

            var entity = _context.Set<T>().Find(id);
            try
            {
                _context.Set<T>().Remove(entity);
                _context.Commit();
                return true;
            }
            catch (Exception)
            {
                _context.Dispose();
                return false;
            }
        }
        public bool DeleteRange(IEnumerable<T> collection)
        {
            _context.BeginTransaction();

            try
            {
                _context.Set<T>().RemoveRange(collection);
                _context.Commit();
                return true;
            }
            catch (Exception)
            {
                _context.Dispose();
                return false;
            }
        }
        public virtual bool Edit(T entity)
        {
            _context.BeginTransaction();
            try
            {
                _context.Set<T>().Update(entity);
                _context.Commit();
                return true;
            }
            catch (Exception ex)
            {
                _context.Rollback();
                return false;
            }
        }
        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Any(predicate);
        }
        public IList<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate).ToList();
        }

        public T FirstBy(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().FirstOrDefault(predicate);
        }

        public IList<T> GetAll(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ? _context.Set<T>().ToList() : _context.Set<T>().Where(predicate).ToList();
        }

        public int GetCount()
        {
            return _context.Set<T>().Count();
        }
        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }
        protected object GetPropertyValue(T obj, string name)
        {
            return obj.GetType().GetProperty(name)?.GetValue(obj, null);
        }


    }
}
