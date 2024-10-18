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

namespace CloudERP.Helpers
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // Database
            container.RegisterType<CloudDBEntities>(new PerResolveLifetimeManager());
            // Account Control
            container.RegisterType<List<AccountControlMV>>();
            // Stored Procedures Code
            container.RegisterType<SP_BalanceSheet>(new TransientLifetimeManager());
            container.RegisterType<SP_GeneralTransaction>(new TransientLifetimeManager());
            container.RegisterType<SP_Dashboard>(new TransientLifetimeManager());
            container.RegisterType<SP_Ledger>(new TransientLifetimeManager());
            container.RegisterType<SP_Purchase>(new TransientLifetimeManager());
            container.RegisterType<SP_Sale>(new TransientLifetimeManager());
            container.RegisterType<SP_TrialBalance>(new TransientLifetimeManager());
            // Services
            container.RegisterType<IEmailService, EmailService>(new TransientLifetimeManager());
            container.RegisterType<MLContext>(new TransientLifetimeManager());
            // Entries
            container.RegisterType<SalaryTransaction>(new TransientLifetimeManager());
            container.RegisterType<GeneralTransactionEntry>(new TransientLifetimeManager());
            container.RegisterType<IncomeStatement>(new TransientLifetimeManager());
            container.RegisterType<PurchaseEntry>(new TransientLifetimeManager());
            container.RegisterType<SaleEntry>(new TransientLifetimeManager());

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}