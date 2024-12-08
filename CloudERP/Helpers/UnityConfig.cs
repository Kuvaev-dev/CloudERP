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
            container.RegisterType<IUserTypeRepository, UserTypeRepository>();
            container.RegisterType<ICategoryRepository, CategoryRepository>();

            // Services
            container.RegisterType<IAccountControlService, AccountControlService>();
            container.RegisterType<IAccountHeadService, AccountHeadService>();
            container.RegisterType<IAccountSubControlService, AccountSubControlService>();
            container.RegisterType<IUserTypeService, UserTypeService>();
            container.RegisterType<ICategoryService, CategoryService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}