using System;

namespace Warsmiths.Common.Domain
{
    public class Lot : IEntity
    {
        #region Ctor

        public Lot()
        {
            Price = 0;
        }

        public void Put(IEntity equipment, int price)
        {
            if (string.IsNullOrEmpty(equipment.OwnerId))
            {
                throw new NullReferenceException("owner is not assigned!");
            }
            Price = price;
            PublishDateTime = DateTime.UtcNow;
            Entity = equipment;
            _id = Guid.NewGuid().ToString();
            OwnerId = equipment.OwnerId;
        }

        #endregion

        #region Props

        public IEntity Entity ;

        public int Price ;

        public DateTime PublishDateTime ;

        #endregion

        #region Methods

        public override string ToString()
        {
            var name = Entity.GetType().Name;
            return string.Format("name:{0}; item id:{1}; price:{2}; owner:{3}: publish datetime:{4}", name, _id, 
                Price, OwnerId, PublishDateTime);
        }

        public bool TryChangePrice(int newPrice)
        {
            if (newPrice <= 0) return false;
            Price = newPrice;
            return true;
        }

        public bool CheckGoldEnough(Player player)
        {
            if (player.Gold >= Price)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}