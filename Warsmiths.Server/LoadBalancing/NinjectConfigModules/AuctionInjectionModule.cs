using Ninject.Modules;

using Warsmiths.Common.Domain;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Framework.DataBaseService;

namespace Warsmiths.Server.NinjectConfigModules
{
    public class AuctionInjectionModule : NinjectModule
    {
        private readonly bool _debug;

        public AuctionInjectionModule(bool debug = false)
        {
            _debug = debug;
        }

        public override void Load()
        {
            Bind(typeof(IRepository<Lot>)).To(typeof(LotRepository));
        }
    }
}