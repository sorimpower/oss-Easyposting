using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EasyPosting.Startup))]
namespace EasyPosting
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
