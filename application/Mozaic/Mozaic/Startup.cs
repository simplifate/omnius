using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Mozaic.Startup))]
namespace Mozaic
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
