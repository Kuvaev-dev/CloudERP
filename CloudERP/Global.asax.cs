using CloudERP.Helpers.Forecasting;
using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Unity;
using Unity.AspNet.Mvc;

namespace CloudERP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Helpers.UnityConfig.RegisterComponents();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var container = new UnityContainer();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        protected void Application_AcquireRequestState(Object sender, EventArgs e)
        {
            string culture = "en-US";
            if (Session["Culture"] != null)
            {
                culture = Session["Culture"].ToString();
            }
            ResourceManagerHelper.SetCulture(culture);
        }
    }
}
