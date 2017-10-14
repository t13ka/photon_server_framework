using Ninject.Modules;

namespace Warsmiths.Server.NinjectConfigModules
{
    using YourGame.Common.Domain;
    using YourGame.DatabaseService.Repositories;
    using YourGame.Server.Framework.DataBaseService;

    public class EconomicInjectionModule : NinjectModule
    {
        private readonly bool _debug;

        public EconomicInjectionModule(bool debug = false)
        {
            _debug = debug;
        }

        public override void Load()
        {
            Bind(typeof(IRepository<Player>)).To(typeof(PlayerRepository));
        }
    }
}