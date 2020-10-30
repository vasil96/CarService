
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public interface IRepositoryBase<T>
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        IQueryable<T> Query();
        void Add(T entity);
        void AddRange(IEnumerable<T> entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteAll();
        void Save();
    }
}
