using DatabaseAccess;
using System.Web.Mvc;
using Unity.AspNet.Mvc;
using Unity;
using CloudERP.Models;
using System.Collections.Generic;
using DatabaseAccess.Code.SP_Code;
using DatabaseAccess.Code;
using Unity.Lifetime;
using Microsoft.ML;
using CloudERP.Helpers.Forecasting;
using DatabaseAccess.Repositories;
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