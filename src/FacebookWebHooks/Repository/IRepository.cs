using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FacebookWebHooks
{
    public interface IRepository<T>
    {
        IList<T> GetAll(Expression<Func<T, bool>> predicate = null);
        IList<T> FindBy(Expression<Func<T, bool>> predicate);
        T FirstBy(Expression<Func<T, bool>> predicate);
        int Add(T entity);
        bool Delete(int id);
        bool Edit(T entity);
        int GetCount();
        T GetById(int id);
        bool DeleteRange(IEnumerable<T> collection);
        bool AddRange(IEnumerable<T> collection);
        bool Any(Expression<Func<T, bool>> predicate);
    }
}
