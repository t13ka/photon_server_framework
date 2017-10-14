namespace YourGame.DatabaseService
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using YourGame.Common.Domain;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T>
        where T : IEntity
    {
        /// <summary>
        /// Create instance
        /// </summary>
        /// <param name="entity"></param>
        void Create(T entity);

        /// <summary>
        /// Update instance
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);

        /// <summary>
        /// Update instance
        /// </summary>
        /// <param name="entity"></param>
        void Replace(T entity);

        /// <summary>
        /// Update instance
        /// </summary>
        /// <param name="id"></param>
        void Delete(string id);

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IList<T> SearchFor(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Get all
        /// </summary>
        /// <returns></returns>
        IList<T> GetAll();

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetById(string id);

        /// <summary>
        /// Get count
        /// </summary>
        /// <returns></returns>
        long Count();
    }
}