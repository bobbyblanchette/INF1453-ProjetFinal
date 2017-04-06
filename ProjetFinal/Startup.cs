using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProjetFinal.Startup))]
namespace ProjetFinal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
