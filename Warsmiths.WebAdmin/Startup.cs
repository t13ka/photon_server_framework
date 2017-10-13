using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Warsmiths.WebAdmin.Startup))]
namespace Warsmiths.WebAdmin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
