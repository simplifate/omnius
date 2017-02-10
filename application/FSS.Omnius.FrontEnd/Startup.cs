using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FSS.Omnius.FrontEnd.Startup))]
namespace FSS.Omnius.FrontEnd
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
