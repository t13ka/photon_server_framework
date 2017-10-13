using Warsmiths.Common.Domain;

namespace Warsmiths.Server.GameServer.Craft.Field
{
    public class CraftCustomController : CraftController
    {
        public CraftCustomController(BaseReciept rec, Player owner) : base(rec, owner)
        {
            Reciept = rec;
            CraftOwner = owner;
        }
    }
}
