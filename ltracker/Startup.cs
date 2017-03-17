using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ltracker.Startup))]
namespace ltracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
