// ReSharper disable InconsistentNaming

namespace YourGame.Common.Domain
{
    public class IEntity
    {
        public string _id;

        public string OwnerId ;

        public string Name ;

        public void AssignOwner(IEntity owner)
        {
            OwnerId = owner._id;
        }
    }
}
