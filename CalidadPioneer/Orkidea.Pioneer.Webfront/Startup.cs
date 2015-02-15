using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Orkidea.Pioneer.Webfront.Startup))]
namespace Orkidea.Pioneer.Webfront
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
