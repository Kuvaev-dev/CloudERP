using DatabaseAccess.Context;
using DatabaseAccess.Helpers;
using DatabaseAccess.Repositories.Account;
using DatabaseAccess.Repositories.Branch;
using DatabaseAccess.Repositories.Company;
using DatabaseAccess.Repositories.Customers;
using DatabaseAccess.Repositories.Employees;
using DatabaseAccess.Repositories.Financial;
using DatabaseAccess.Repositories.Inventory;
using DatabaseAccess.Repositories.Purchase;
using DatabaseAccess.Repositories.Sale;
using DatabaseAccess.Repositories.Suppliers;
using DatabaseAccess.Repositories.Users;
using DatabaseAccess.Repositories.Utilities;
using Domain.RepositoryAccess;
using Domain.UtilsAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseAccess.DependencyInjection
{
    public static class DatabaseModule
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
        {
            // Main DB Context
            services.AddScoped<CloudDBEntities>();

            // Database Query Helper
            services.AddScoped<IDatabaseQuery, DatabaseQuery>();
            // Branch Helper
            services.AddScoped<IBranchHelper, BranchHelper>();

            // Account
            services.AddScoped<IAccountActivityRepository, AccountActivityRepository>();
            services.AddScoped<IAccountControlRepository, AccountControlRepository>();
            services.AddScoped<IAccountHeadRepository, AccountHeadRepository>();
            services.AddScoped<IAccountSettingRepository, AccountSettingRepository>();
            services.AddScoped<IAccountSubControlRepository, AccountSubControlRepository>();
            // Balance Sheet
            services.AddScoped<IBalanceSheetRepository, BalanceSheetRepository>();
            // Branch
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IBranchTypeRepository, BranchTypeRepository>();
            // Category
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            // Company
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            // Customer
            services.AddScoped<ICustomerInvoiceRepository, CustomerInvoiceRepository>();
            services.AddScoped<ICustomerInvoiceDetailRepository, CustomerInvoiceDetailRepository>();
            services.AddScoped<ICustomerPaymentRepository, CustomerPaymentRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerReturnInvoiceDetailRepository, CustomerReturnInvoiceDetailRepository>();
            services.AddScoped<ICustomerReturnInvoiceRepository, CustomerReturnInvoiceRepository>();
            services.AddScoped<ICustomerReturnPaymentRepository, CustomerReturnPaymentRepository>();
            // Dashboard
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            // Employee
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            // Financial Yaer
            services.AddScoped<IFinancialYearRepository, FinancialYearRepository>();
            // Forecasting
            services.AddScoped<IForecastingRepository, ForecastingRepository>();
            // General Transaction
            services.AddScoped<IGeneralTransactionRepository, GeneralTransactionRepository>();
            // Ledger
            services.AddScoped<ILedgerRepository, LedgerRepository>();
            // Payroll
            services.AddScoped<IPayrollRepository, PayrollRepository>();
            // Purchase
            services.AddScoped<IPurchaseCartDetailRepository, PurchaseCartDetailRepository>();
            services.AddScoped<IPurchaseRepository, PurchaseRepository>();
            // Salary Transaction
            services.AddScoped<ISalaryTransactionRepository, SalaryTransactionRepository>();
            // Sale
            services.AddScoped<ISaleCartDetailRepository, SaleCartDetailRepository>();
            services.AddScoped<ISaleRepository, SaleRepository>();
            // Stock
            services.AddScoped<IStockRepository, StockRepository>();
            // Supplier
            services.AddScoped<ISupplierInvoiceDetailRepository, SupplierInvoiceDetailRepository>();
            services.AddScoped<ISupplierInvoiceRepository, SupplierInvoiceRepository>();
            services.AddScoped<ISupplierPaymentRepository, SupplierPaymentRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<ISupplierReturnInvoiceDetailRepository, SupplierReturnInvoiceDetailRepository>();
            services.AddScoped<ISupplierReturnInvoiceRepository, SupplierReturnInvoiceRepository>();
            services.AddScoped<ISupplierReturnPaymentRepository, SupplierReturnPaymentRepository>();
            // Support
            services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();
            // Task
            services.AddScoped<ITaskRepository, TaskRepository>();
            // Trial Balance
            services.AddScoped<ITrialBalanceRepository, TrialBalanceRepository>();
            // User
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserTypeRepository, UserTypeRepository>();

            return services;
        }
    }
}
