using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Warsmiths.Common.Domain;
using Warsmiths.Server.Framework.DataBaseService;

namespace Warsmiths.DatabaseService.Repositories.Mockup
{
    public class BaseFakeRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly List<T> _collection;

        public BaseFakeRepository()
        {
            _collection = new List<T>();
        }

        public virtual void Create(T entity)
        {
            _collection.Add(entity);
        }

        public virtual void Update(T entity)
        {
            var existedEntity = _collection.FirstOrDefault(t => t._id == entity._id);
            if (existedEntity != null)
            {
                existedEntity = entity;
            }
        }

        public virtual void Replace(T entity)
        {
            var existedEntity = _collection.FirstOrDefault(t => t._id == entity._id);
            if (existedEntity != null)
            {
                _collection.Remove(existedEntity);
            }
            _collection.Add(entity);
        }

        public virtual void Delete(string id)
        {
            var existedEntity = _collection.FirstOrDefault(t => t._id == id);
            if (existedEntity != null)
            {
                _collection.Remove(existedEntity);
            }
        }

        public IList<T> SearchFor(Expression<Func<T, bool>> predicate)
        {
            var r = _collection.AsQueryable()
                .Where(predicate.Compile())
                .ToList();
            return r;
        }

        public virtual IList<T> GetAll()
        {
            return _collection;
        }

        public virtual T GetById(string id)
        {
            return _collection.FirstOrDefault(t => t._id == id);
        }

        public virtual long Count()
        {
            return _collection.Count;
        }
    }
}