using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Unity;

namespace API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static IUnityContainer _container;

        protected void Application_Start()
        {
            _container = new UnityContainer();

            Helpers.UnityConfig.RegisterComponents(_container);
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
