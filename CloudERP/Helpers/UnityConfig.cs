using Unity.AspNet.Mvc;
using Unity;
using System.Web;
using System.Web.Mvc;
using Utils.Interfaces;

namespace CloudERP.Helpers
{
    public static class UnityConfig
    {
        public static void RegisterComponents(IUnityContainer container)
        {
            #region Providers
            // DB Connection String
            container.RegisterType<IConnectionStringProvider, WebConfigConnectionStringProvider>();
            #endregion

            #region Helpers
            // Session Helper
            container.RegisterFactory<SessionHelper>(c => new SessionHelper(new HttpSessionStateWrapper(HttpContext.Current.Session)));
            container.RegisterType<HttpClientHelper>();
            #endregion

            #region Services
            container.RegisterType<IEmailService, EmailService>();
            container.RegisterType<ICurrencyService, CurrencyService>();
            #endregion

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}