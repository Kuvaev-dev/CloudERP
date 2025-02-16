using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Unity;
using Utils.Helpers;

namespace CloudERP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static IUnityContainer _container;

        protected void Application_Start()
        {
            _container = new UnityContainer();

            Helpers.UnityConfig.RegisterComponents(_container);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AcquireRequestState(Object sender, EventArgs e)
        {
            var resourceManager = _container.Resolve<ResourceManagerHelper>();
            string culture = "en-US";

            if (Session?["Culture"] != null)
            {
                culture = Session["Culture"].ToString();
            }
            resourceManager.SetCulture(culture);
        }
    }
}
