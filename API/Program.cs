using API.Factories;
using API.Helpers;
using API.Services;
using DatabaseAccess.Context;
using DatabaseAccess.Helpers;
using DatabaseAccess.Repositories.Account;
using DatabaseAccess.Repositories.Branch;
using DatabaseAccess.Repositories.Company;
using DatabaseAccess.Repositories.Customers;
using DatabaseAccess.Repositories.Employees;
using DatabaseAccess.Repositories.Finance;
using DatabaseAccess.Repositories.Inventory;
using DatabaseAccess.Repositories.Purchase;
using DatabaseAccess.Repositories.Sale;
using DatabaseAccess.Repositories.Suppliers;
using DatabaseAccess.Repositories.Users;
using DatabaseAccess.Repositories.Utilities;
using Domain.Facades;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ML;
using Services.Adapters;
using Services.Facades;
using Services.Implementations;
using Services.ServiceAccess;
using System.Text;
using Utils.Helpers;
using Utils.Interfaces;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            #region Context
            // Main DB Context
            builder.Services.AddScoped<CloudDBEntities>();
            // ML Context
            builder.Services.AddSingleton<MLContext>();
            #endregion

            #region Helpers
            // Database Query Helper
            builder.Services.AddScoped<DatabaseQuery>();
            // Branch Helper
            builder.Services.AddScoped<BranchHelper>();
            // Password Helper
            builder.Services.AddScoped<PasswordHelper>();
            // Culture Helper
            builder.Services.AddScoped<ResourceManagerHelper>();
            // Connetion String Provider
            builder.Services.AddScoped<IConnectionStringProvider, WebConfigConnectionStringProvider>();
            #endregion

            #region Facades
            // Domain
            builder.Services.AddScoped<PurchaseCartFacade>();
            builder.Services.AddScoped<PurchasePaymentFacade>();
            builder.Services.AddScoped<PurchaseReturnFacade>();
            builder.Services.AddScoped<SalaryTransactionFacade>();
            builder.Services.AddScoped<SaleCartFacade>();
            builder.Services.AddScoped<SaleEntryFacade>();
            // Cloud ERP
            builder.Services.AddScoped<AccountSettingFacade>();
            builder.Services.AddScoped<CompanyEmployeeFacade>();
            builder.Services.AddScoped<CompanyRegistrationFacade>();
            builder.Services.AddScoped<HomeFacade>();
            #endregion

            #region Repositories
            // Account
            builder.Services.AddScoped<IAccountActivityRepository, AccountActivityRepository>();
            builder.Services.AddScoped<IAccountControlRepository, AccountControlRepository>();
            builder.Services.AddScoped<IAccountHeadRepository, AccountHeadRepository>();
            builder.Services.AddScoped<IAccountSettingRepository, AccountSettingRepository>();
            builder.Services.AddScoped<IAccountSubControlRepository, AccountSubControlRepository>();
            // Balance Sheet
            builder.Services.AddScoped<IBalanceSheetRepository, BalanceSheetRepository>();
            // Branch
            builder.Services.AddScoped<IBranchRepository, BranchRepository>();
            builder.Services.AddScoped<IBranchTypeRepository, BranchTypeRepository>();
            // Category
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            // Company
            builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
            // Customer
            builder.Services.AddScoped<ICustomerInvoiceRepository, CustomerInvoiceRepository>();
            builder.Services.AddScoped<ICustomerInvoiceDetailRepository, CustomerInvoiceDetailRepository>();
            builder.Services.AddScoped<ICustomerPaymentRepository, CustomerPaymentRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<ICustomerReturnInvoiceDetailRepository, CustomerReturnInvoiceDetailRepository>();
            builder.Services.AddScoped<ICustomerReturnInvoiceRepository, CustomerReturnInvoiceRepository>();
            builder.Services.AddScoped<ICustomerReturnPaymentRepository, CustomerReturnPaymentRepository>();
            // Dashboard
            builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
            // Employee
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            // Financial Yaer
            builder.Services.AddScoped<IFinancialYearRepository, FinancialYearRepository>();
            // Forecasting
            builder.Services.AddScoped<IForecastingRepository, ForecastingRepository>();
            // General Transaction
            builder.Services.AddScoped<IGeneralTransactionRepository, GeneralTransactionRepository>();
            // Ledger
            builder.Services.AddScoped<ILedgerRepository, LedgerRepository>();
            // Payroll
            builder.Services.AddScoped<IPayrollRepository, PayrollRepository>();
            // Purchase
            builder.Services.AddScoped<IPurchaseCartDetailRepository, PurchaseCartDetailRepository>();
            builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
            // Salary Transaction
            builder.Services.AddScoped<ISalaryTransactionRepository, SalaryTransactionRepository>();
            // Sale
            builder.Services.AddScoped<ISaleCartDetailRepository, SaleCartDetailRepository>();
            builder.Services.AddScoped<ISaleRepository, SaleRepository>();
            // Stock
            builder.Services.AddScoped<IStockRepository, StockRepository>();
            // Supplier
            builder.Services.AddScoped<ISupplierInvoiceDetailRepository, SupplierInvoiceDetailRepository>();
            builder.Services.AddScoped<ISupplierInvoiceRepository, SupplierInvoiceRepository>();
            builder.Services.AddScoped<ISupplierPaymentRepository, SupplierPaymentRepository>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<ISupplierReturnInvoiceDetailRepository, SupplierReturnInvoiceDetailRepository>();
            builder.Services.AddScoped<ISupplierReturnInvoiceRepository, SupplierReturnInvoiceRepository>();
            builder.Services.AddScoped<ISupplierReturnPaymentRepository, SupplierReturnPaymentRepository>();
            // Support
            builder.Services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();
            // Task
            builder.Services.AddScoped<ITaskRepository, TaskRepository>();
            // Trial Balance
            builder.Services.AddScoped<ITrialBalanceRepository, TrialBalanceRepository>();
            // User
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserTypeRepository, UserTypeRepository>();
            #endregion

            #region Services
            // Main
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IBalanceSheetService, BalanceSheetService>();
            builder.Services.AddScoped<ICurrencyService, CurrencyService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IEmployeeSalaryService, EmployeeSalaryService>();
            builder.Services.AddScoped<IEmployeeStatisticsService, EmployeeStatisticsService>();
            builder.Services.AddScoped<IGeneralTransactionService, GeneralTransactionService>();
            builder.Services.AddScoped<IIncomeStatementService, IncomeStatementService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IProductQualityService, ProductQualityService>();
            builder.Services.AddScoped<IReminderService, ReminderService>();
            builder.Services.AddScoped<ISalaryTransactionService, SalaryTransactionService>();
            // Purchase
            builder.Services.AddScoped<IPurchaseCartService, PurchaseCartService>();
            builder.Services.AddScoped<IPurchaseEntryService, PurchaseEntryService>();
            builder.Services.AddScoped<IPurchasePaymentReturnService, PurchasePaymentReturnService>();
            builder.Services.AddScoped<IPurchasePaymentService, PurchasePaymentService>();
            builder.Services.AddScoped<IPurchaseReturnService, PurchaseReturnService>();
            builder.Services.AddScoped<IPurchaseService, PurchaseService>();
            // Sale
            builder.Services.AddScoped<ISaleCartService, SaleCartService>();
            builder.Services.AddScoped<ISaleEntryService, SaleEntryService>();
            builder.Services.AddScoped<ISalePaymentReturnService, SalePaymentReturnService>();
            builder.Services.AddScoped<ISalePaymentService, SalePaymentService>();
            builder.Services.AddScoped<ISaleReturnService, SaleReturnService>();
            builder.Services.AddScoped<ISaleService, SaleService>();
            // Miscellaneous
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IForecastingService, ForecastingService>();
            builder.Services.AddScoped<IPurchaseEntryService, PurchaseEntryService>();
            builder.Services.AddScoped<ISaleEntryService, SaleEntryService>();
            builder.Services.AddScoped<IFinancialForecaster, FinancialForecaster>();
            #endregion

            #region Adapters
            // Forecasting
            builder.Services.AddScoped<IFinancialForecastAdapter, FinancialForecastAdapter>();
            // File Upload
            builder.Services.AddScoped<IFileAdapterFactory, FileAdapterFactory>();
            #endregion

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
