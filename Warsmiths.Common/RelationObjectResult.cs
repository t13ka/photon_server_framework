using Warsmiths.Common.Domain;

namespace Warsmiths.Common
{
    public class RelationObjectResult
    {
        public IEntity Parent { get; private set; }
        public IEntity Child { get; private set; }

        public RelationObjectResult(IEntity parent, IEntity child)
        {
            Parent = parent;
            Child = child;
        }
    }
}
