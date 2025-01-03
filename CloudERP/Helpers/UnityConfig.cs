using DatabaseAccess;
using DatabaseAccess.Repositories;
using System.Web.Mvc;
using Unity.AspNet.Mvc;
using Unity;
using Unity.Injection;
using System.Web;
using Domain.RepositoryAccess;
using Domain.Services;
using DatabaseAccess.Services;
using DatabaseAccess.Helpers;
using System.Configuration;
using Domain.Services.Purchase;
using Domain.Services.Sale;
using DatabaseAccess.Adapters;
using Domain.Facades;
using CloudERP.Facades;
using CloudERP.Factories;
using Domain.Interfaces;

namespace CloudERP.Helpers
{
    public static class UnityConfig
    {
        public static void RegisterComponents(IUnityContainer container)
        {
            #region DbContext
            // Main DB Context
            container.RegisterType<CloudDBEntities>();
            #endregion

            #region Helpers
            // Session Helper
            container.RegisterFactory<SessionHelper>(c => new SessionHelper(new HttpSessionStateWrapper(HttpContext.Current.Session)));
            // Database Query Helper
            container.RegisterType<DatabaseQuery>(new InjectionConstructor(ConfigurationManager.ConnectionStrings["CloudDBEntities"].ConnectionString));
            // Branch Helper
            container.RegisterType<BranchHelper>();
            // Password Helper
            container.RegisterType<PasswordHelper>();
            // Culture Helper
            container.RegisterType<ResourceManagerHelper>();
            #endregion

            #region Facades
            // Domain
            container.RegisterType<PurchaseCartFacade>();
            container.RegisterType<PurchasePaymentFacade>();
            container.RegisterType<PurchaseReturnFacade>();
            container.RegisterType<SalaryTransactionFacade>();
            container.RegisterType<SaleCartFacade>();
            container.RegisterType<SaleEntryFacade>();
            // Cloud ERP
            container.RegisterType<AccountSettingFacade>();
            container.RegisterType<CompanyEmployeeFacade>();
            container.RegisterType<CompanyRegistrationFacade>();
            container.RegisterType<HomeFacade>();
            #endregion

            #region Repositories
            // Account
            container.RegisterType<IAccountActivityRepository, AccountActivityRepository>();
            container.RegisterType<IAccountControlRepository, AccountControlRepository>();
            container.RegisterType<IAccountHeadRepository, AccountHeadRepository>();
            container.RegisterType<IAccountSettingRepository, AccountSettingRepository>();
            container.RegisterType<IAccountSubControlRepository, AccountSubControlRepository>();
            // Balance Sheet
            container.RegisterType<IBalanceSheetRepository, BalanceSheetRepository>();
            // Branch
            container.RegisterType<IBranchRepository, BranchRepository>();
            container.RegisterType<IBranchTypeRepository, BranchTypeRepository>();
            // Category
            container.RegisterType<ICategoryRepository, CategoryRepository>();
            // Company
            container.RegisterType<ICompanyRepository, CompanyRepository>();
            // Customer
            container.RegisterType<ICustomerInvoiceDetailRepository, CustomerInvoiceDetailRepository>();
            container.RegisterType<ICustomerPaymentRepository, CustomerPaymentRepository>();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            container.RegisterType<ICustomerReturnInvoiceDetailRepository, CustomerReturnInvoiceDetailRepository>();
            container.RegisterType<ICustomerReturnInvoiceRepository, CustomerReturnInvoiceRepository>();
            container.RegisterType<ICustomerReturnPaymentRepository, CustomerReturnPaymentRepository>();
            // Dashboard
            container.RegisterType<IDashboardRepository, DashboardRepository>();
            // Employee
            container.RegisterType<IEmployeeRepository, EmployeeRepository>();
            // Financial Yaer
            container.RegisterType<IFinancialYearRepository, FinancialYearRepository>();
            // Forecasting
            container.RegisterType<IForecastingRepository, ForecastingRepository>();
            // General Transaction
            container.RegisterType<IGeneralTransactionRepository, GeneralTransactionRepository>();
            // Ledger
            container.RegisterType<ILedgerRepository, LedgerRepository>();
            // Payroll
            container.RegisterType<IPayrollRepository, PayrollRepository>();
            // Purchase
            container.RegisterType<IPurchaseCartDetailRepository, PurchaseCartDetailRepository>();
            container.RegisterType<IPurchaseRepository, PurchaseRepository>();
            // Salary Transaction
            container.RegisterType<ISalaryTransactionRepository, SalaryTransactionRepository>();
            // Sale
            container.RegisterType<ISaleCartDetailRepository, SaleCartDetailRepository>();
            container.RegisterType<ISaleRepository, SaleRepository>();
            // Stock
            container.RegisterType<IStockRepository, StockRepository>();
            // Supplier
            container.RegisterType<ISupplierInvoiceDetailRepository, SupplierInvoiceDetailRepository>();
            container.RegisterType<ISupplierInvoiceRepository, SupplierInvoiceRepository>();
            container.RegisterType<ISupplierPaymentRepository, SupplierPaymentRepository>();
            container.RegisterType<ISupplierRepository, SupplierRepository>();
            container.RegisterType<ISupplierReturnInvoiceDetailRepository, SupplierReturnInvoiceDetailRepository>();
            container.RegisterType<ISupplierReturnInvoiceRepository, SupplierReturnInvoiceRepository>();
            container.RegisterType<ISupplierReturnPaymentRepository, SupplierReturnPaymentRepository>();
            // Support
            container.RegisterType<ISupportTicketRepository, SupportTicketRepository>();
            // Task
            container.RegisterType<ITaskRepository, TaskRepository>();
            // Trial Balance
            container.RegisterType<ITrialBalanceRepository, TrialBalanceRepository>();
            // User
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IUserTypeRepository, UserTypeRepository>();
            #endregion

            #region Services
            // Main
            container.RegisterType<IAuthService, AuthService>();
            container.RegisterType<IBalanceSheetService, BalanceSheetService>();
            container.RegisterType<IDashboardService, DashboardService>();
            container.RegisterType<IEmployeeSalaryService, EmployeeSalaryService>();
            container.RegisterType<IEmployeeStatisticsService, EmployeeStatisticsService>();
            container.RegisterType<IGeneralTransactionService, GeneralTransactionService>();
            container.RegisterType<IIncomeStatementService, IncomeStatementService>();
            container.RegisterType<IReminderService, ReminderService>();
            container.RegisterType<ISalaryTransactionService, SalaryTransactionService>();
            // Purchase
            container.RegisterType<IPurchaseCartService, PurchaseCartService>();
            container.RegisterType<IPurchaseEntryService, PurchaseEntryService>();
            container.RegisterType<IPurchasePaymentReturnService, PurchasePaymentReturnService>();
            container.RegisterType<IPurchasePaymentService, PurchasePaymentService>();
            container.RegisterType<IPurchaseReturnService, PurchaseReturnService>();
            // Sale
            container.RegisterType<ISaleCartService, SaleCartService>();
            container.RegisterType<ISaleEntryService, SaleEntryService>();
            container.RegisterType<ISalePaymentReturnService, SalePaymentReturnService>();
            container.RegisterType<ISalePaymentService, SalePaymentService>();
            container.RegisterType<ISaleReturnService, SaleReturnService>();
            // Miscellaneous
            container.RegisterType<IFileService, FileService>();
            container.RegisterType<IEmailService, EmailService>();
            container.RegisterType<IForecastingService, ForecastingService>();
            container.RegisterType<IPurchaseEntryService, PurchaseEntryService>();
            container.RegisterType<ISaleEntryService, SaleEntryService>();
            container.RegisterType<IFinancialForecaster, FinancialForecaster>();
            #endregion

            #region Adapters
            // Forecasting
            container.RegisterType<IFinancialForecastAdapter, FinancialForecastAdapter>();
            // File Upload
            container.RegisterType<IFile, HttpPostedFileAdapter>();
            container.RegisterType<IFileAdapterFactory, FileAdapterFactory>();
            #endregion

            #region Providers
            // DB Connection String
            container.RegisterType<IConnectionStringProvider, WebConfigConnectionStringProvider>();
            #endregion

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}