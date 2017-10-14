using Ninject.Modules;

namespace YourGame.Server.NinjectConfigModules
{
    using YourGame.Common.Domain;
    using YourGame.DatabaseService.Repositories;
    using YourGame.Server.Framework.DataBaseService;

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