using DatabaseAccess.Repositories;
using Domain.RepositoryAccess;
using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;

namespace API.Helpers
{
    public static class UnityConfig
    {
        public static void RegisterComponents(IUnityContainer container)
        {
            container.RegisterType<IAccountHeadRepository, AccountHeadRepository>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}