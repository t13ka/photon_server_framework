using ExitGames.Logging;
using Ninject.Modules;
using Warsmiths.Common.Domain;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.DatabaseService.Repositories.Mockup;
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
            if (!_debug)
            {
                LogManager.GetCurrentClassLogger().Debug("aaaa");
                Bind(typeof(IRepository<Lot>)).To(typeof(LotRepository));
                LogManager.GetCurrentClassLogger().Debug("bbb");
            }
            else
            {
                Bind(typeof(IRepository<Lot>)).To(typeof(FakePlayerRepository));
            }
        }
    }
}
