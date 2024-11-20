using DatabaseAccess;
using DatabaseAccess.Repositories;
using System.Web.Mvc;
using Unity.AspNet.Mvc;
using Unity;
using Unity.Lifetime;
using Domain.Services;

namespace CloudERP.Helpers
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // DbContext
            container.RegisterType<CloudDBEntities>(new PerResolveLifetimeManager());

            // Repos
            container.RegisterType<IAccountControlRepository, AccountControlRepository>();

            // Services
            container.RegisterType<IAccountControlService, AccountControlService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}