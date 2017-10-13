using System;
using System.Collections.Generic;

namespace Warsmiths.Common.Domain
{
    public class PlayerInventory : Dictionary<string, IEntity>
    {
        private const int MaximumItems = 300;

        public bool Add(InventoryItem item)
        {
            if (string.IsNullOrEmpty(item.Entity._id))
            {
                throw new NullReferenceException("An item should have an unique identifier!");
            }
            
            if (item.Entity is Lot)
            {
                throw new NullReferenceException("cant add the LOT!");
            }

            bool result;

            if (Count >= MaximumItems)
            {
                result = false;
            }
            else
            {
                if (ContainsKey(item.Entity._id) == false)
                {
                    Add(item.Entity._id, item.Entity);
                    item.Entity.AssignOwner(item.Owner);
                    result = true;
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }
      
        public bool TryPack(InventoryItem obj)
        {
            bool result;

            if (ContainsKey(obj.Entity._id) == false)
            {
                Add(obj);
                result = true;
            }
            else
            {
                var baseElement = obj.Entity as BaseElement;
                if (baseElement != null)
                {
                    IEntity entity;

                    if (TryGetValue(baseElement._id, out entity))
                    {
                        var element = (BaseElement) entity;

                        element.Quantity = element.Quantity + baseElement.Quantity;

                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        public bool TryUnpack(InventoryItem obj)
        {
            bool result;

            var baseElement = obj.Entity as BaseElement;
            if (baseElement != null)
            {
                IEntity item;
                if (TryGetValue(baseElement._id, out item))
                {
                    var element = (BaseElement) item;

                    element.Quantity = element.Quantity - baseElement.Quantity;

                    if (element.Quantity <= 0)
                    {
                        Remove(item._id);
                    }

                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }
    }
}