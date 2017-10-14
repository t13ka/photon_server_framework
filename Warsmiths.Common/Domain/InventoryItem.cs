namespace YourGame.Common.Domain
{
    public class InventoryItem
    {
        public IEntity Entity { get; }
        public IEntity Owner { get; }

        public InventoryItem(IEntity entity, IEntity owner)
        {
            Entity = entity;
            Owner = owner;
        }
    }
}