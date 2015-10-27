using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FSPOC_WebProject.Startup))]
namespace FSPOC_WebProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
