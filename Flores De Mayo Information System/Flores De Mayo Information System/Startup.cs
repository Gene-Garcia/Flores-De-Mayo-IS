using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Flores_De_Mayo_Information_System.Startup))]
namespace Flores_De_Mayo_Information_System
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
