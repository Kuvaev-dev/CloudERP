using DatabaseAccess;
using System.Web.Mvc;
using Unity.AspNet.Mvc;
using Unity;

namespace CloudERP.Helpers
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            container.RegisterType<CloudDBEntities>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}