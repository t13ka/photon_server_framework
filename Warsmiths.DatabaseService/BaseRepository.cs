using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Equipment;
using Warsmiths.Server.Framework.DataBaseService;

namespace Warsmiths.DatabaseService
{
    public class BaseRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> _collection;

        protected BaseRepository()
        {
            var client = new MongoClient(DataBaseService.Default.DatabaseConnectionString);
            var database = client.GetDatabase(DataBaseService.Default.DataBaseInternalName);
            _collection = database.GetCollection<T>(typeof (T).Name.ToLower() + "s");
        }

        public virtual void Create(T entity)
        {
            _collection.InsertOne(entity);
        }

        /// <summary>
        /// To update specific property(ies) you must override this method
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(T entity)
        {
            //var filter = Builders<Player>.Filter.Eq("_id", entity._id);
            //        var update = Builders<Player>.Update
            //            .Set(x => x.Inventory, entity.PlayerInventory)
            //            .Set(x => x.Age, entity.Age)
            //            .Set(x => x.Characters, entity.Characters)
            //            .Set(x => x.Banned, entity.Banned)
            //            .Set(x => x.Crystals, entity.Crystals)
            //            .Set(x => x.CurrentClan, entity.CurrentClan)
            //            .Set(x => x.CurrentLeague, entity.CurrentLeague)
            //            .Set(x => x.Email, entity.Email)
            //            .Set(x => x.Gold, entity.Gold)
            //            .Set(x => x.Online, entity.Online)
            //            .Set(x => x.GenderType, entity.GenderType);

            //        DataBaseHandler.Collection.UpdateOne(filter, update);

            _collection.ReplaceOne(t => t._id == entity._id, entity);
        }

        public virtual void Replace(T entity)
        {
            _collection.ReplaceOne(t => t._id == entity._id, entity);
        }

        public virtual void Delete(string id)
        {
            _collection.DeleteOne(t => t._id == id);
        }

        public virtual IList<T> SearchFor(Expression<Func<T, bool>> predicate)
        {
            var r = _collection.AsQueryable()
                .Where(predicate.Compile())
                .ToList();
            return r;
        }
        public virtual IList<T> GetAllByOwner(string ownerId)
        {
            return _collection.FindSync(t => t.OwnerId == ownerId).ToList();
        }

        public virtual IList<T> GetAll()
        {
            return _collection.FindSync(new BsonDocument()).ToList();
        }

        public virtual T GetById(string id)
        {
            return _collection.FindSync(t => t._id == id).ToEnumerable().FirstOrDefault();
        }

        public virtual long Count()
        {
            return _collection.Count(new BsonDocument());
        }
    }
}