using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AdaptiveSystems.AspNetIdentity.OAuth;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebApi2WithTokenAuthentication.Startup))]

namespace WebApi2WithTokenAuthentication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //MVC related startup
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //WebApi related startup
            var config = new HttpConfiguration();

            StartupOptions.ConfigureOAuth(app);

            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);

            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
