using DatabaseAccess;
using DatabaseAccess.Repositories;
using System.Web.Mvc;
using Unity.AspNet.Mvc;
using Unity;
using Unity.Lifetime;
using Domain.Services;
using Unity.Injection;

namespace CloudERP.Helpers
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // DbContext
            container.RegisterType<CloudDBEntities>(
                new InjectionFactory(c => new CloudDBEntities())
            );

            // Repos
            container.RegisterType<IAccountControlRepository, AccountControlRepository>();
            container.RegisterType<IAccountHeadRepository, AccountHeadRepository>();
            container.RegisterType<IAccountSubControlRepository, AccountSubControlRepository>();

            // Services
            container.RegisterType<IAccountControlService, AccountControlService>();
            container.RegisterType<IAccountHeadService, AccountHeadService>();
            container.RegisterType<IAccountSubControlService, AccountSubControlService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}