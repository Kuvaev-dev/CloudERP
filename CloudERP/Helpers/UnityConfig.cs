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

            // Session
            container.RegisterType<SessionHelper>(
                new InjectionFactory(c => new SessionHelper(new HttpSessionStateWrapper(HttpContext.Current.Session)))
            );

            // Repositories
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

            // Services
            container.RegisterType<IBalanceSheetService, BalanceSheetService>();
            container.RegisterType<IEmployeeSalaryService, EmployeeSalaryService>();
            container.RegisterType<IEmployeeStatisticsService, EmployeeStatisticsService>();
            container.RegisterType<IFileService, FileService>();
            container.RegisterType<IEmailService, EmailService>();
            container.RegisterType<IForecastingService, ForecastingService>();
            container.RegisterType<IIncomeStatementService, IncomeStatementService>();
            container.RegisterType<IPurchaseEntryService, PurchaseEntryService>();
            container.RegisterType<IReminderService, ReminderService>();
            container.RegisterType<ISalaryTransactionService, SalaryTransactionService>();
            container.RegisterType<ISaleEntryService, SaleEntryService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}