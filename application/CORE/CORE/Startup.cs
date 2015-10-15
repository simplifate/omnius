using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CORE.Startup))]
namespace CORE
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
