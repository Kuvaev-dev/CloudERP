using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CloudERP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            Helpers.UnityConfig.RegisterComponents();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AcquireRequestState(Object sender, EventArgs e)
        {
            string culture = "en-US"; // Default culture
            if (Session["Culture"] != null)
            {
                culture = Session["Culture"].ToString();
            }
            ResourceManagerHelper.SetCulture(culture);
        }
    }
}
