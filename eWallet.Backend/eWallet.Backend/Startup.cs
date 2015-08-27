using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(eWallet.Backend.Startup))]
namespace eWallet.Backend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
