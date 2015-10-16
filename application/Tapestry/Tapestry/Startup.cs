using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Tapestry.Startup))]
namespace Tapestry
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
