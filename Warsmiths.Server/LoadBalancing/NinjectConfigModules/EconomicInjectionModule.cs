using Ninject.Modules;
using Warsmiths.Common.Domain;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.DatabaseService.Repositories.Mockup;
using Warsmiths.Server.Framework.DataBaseService;

namespace Warsmiths.Server.NinjectConfigModules
{
    public class EconomicInjectionModule : NinjectModule
    {
        private readonly bool _debug;

        public EconomicInjectionModule(bool debug = false)
        {
            _debug = debug;
        }

        public override void Load()
        {
            if (!_debug)
            {
                Bind(typeof(IRepository<Player>)).To(typeof(PlayerRepository));
            }
            else
            {
                Bind(typeof(IRepository<Player>)).To(typeof(FakePlayerRepository));
            }
        }
    }
}
