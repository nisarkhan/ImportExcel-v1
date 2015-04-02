using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ImportExcel_v1.Startup))]
namespace ImportExcel_v1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
