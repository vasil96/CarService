using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected RepositoryContext RepositoryContext { get; set; }

        public RepositoryBase(RepositoryContext repositoryContext)
        {
            this.RepositoryContext = repositoryContext;
        }

        public T GetById(int id)
        {
            return this.RepositoryContext.Set<T>().Find(id);
        }
        public IEnumerable<T> GetAll()
        {
            return this.RepositoryContext.Set<T>();
        }
        public IQueryable<T> Query()
        {
            return this.RepositoryContext.Set<T>();
        }
        public void Add(T entity)
        {
            this.RepositoryContext.Set<T>().Add(entity);
        }
        public void AddRange(IEnumerable<T> entity)
        {
            this.RepositoryContext.Set<T>().AddRange(entity);
        }
        public void Update(T entity)
        {
            this.RepositoryContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            this.RepositoryContext.Set<T>().Remove(entity);
        }
        public void DeleteAll()
        {
            this.RepositoryContext.Set<T>().RemoveRange(this.RepositoryContext.Set<T>());
        }

        public void Save()
        {
            this.RepositoryContext.SaveChanges();
        }
    }
}
