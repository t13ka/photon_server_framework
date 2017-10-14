using Ninject.Modules;

namespace YourGame.Server.NinjectConfigModules
{
    using YourGame.Common.Domain;
    using YourGame.DatabaseService;
    using YourGame.DatabaseService.Repositories;

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