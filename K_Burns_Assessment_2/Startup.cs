using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(K_Burns_Assessment_2.Startup))]
namespace K_Burns_Assessment_2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
