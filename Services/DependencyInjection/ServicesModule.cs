using Domain.Facades;
using Domain.ServiceAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML;
using Services.Adapters;
using Services.Facades;
using Services.Implementations;
using Utils.Interfaces;

namespace Services.DependencyInjection
{
    public static class ServicesModule
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Adapters
            services.AddScoped<IFinancialForecastAdapter, FinancialForecastAdapter>();

            // Facades
            services.AddScoped<AccountSettingFacade>();
            services.AddScoped<CompanyEmployeeFacade>();
            services.AddScoped<CompanyRegistrationFacade>();
            services.AddScoped<HomeFacade>();
            services.AddScoped<PurchaseCartFacade>();
            services.AddScoped<PurchasePaymentFacade>();
            services.AddScoped<PurchaseReturnFacade>();
            services.AddScoped<PurchaseEntryFacade>();
            services.AddScoped<SalaryTransactionFacade>();
            services.AddScoped<SaleCartFacade>();
            services.AddScoped<SalePaymentFacade>();
            services.AddScoped<SaleEntryFacade>();
            services.AddScoped<SaleReturnFacade>();

            // ML
            services.AddScoped<MLContext>();

            // Services
            // Main
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBalanceSheetService, BalanceSheetService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IEmployeeSalaryService, EmployeeSalaryService>();
            services.AddScoped<IEmployeeStatisticsService, EmployeeStatisticsService>();
            services.AddScoped<IGeneralTransactionService, GeneralTransactionService>();
            services.AddScoped<IIncomeStatementService, IncomeStatementService>();
            services.AddScoped<IProductQualityService, ProductQualityService>();
            services.AddScoped<IReminderService, ReminderService>();
            services.AddScoped<ISalaryTransactionService, SalaryTransactionService>();
            // Purchase
            services.AddScoped<IPurchaseCartService, PurchaseCartService>();
            services.AddScoped<IPurchaseEntryService, PurchaseEntryService>();
            services.AddScoped<IPurchasePaymentReturnService, PurchasePaymentReturnService>();
            services.AddScoped<IPurchasePaymentService, PurchasePaymentService>();
            services.AddScoped<IPurchaseReturnService, PurchaseReturnService>();
            services.AddScoped<IPurchaseService, PurchaseService>();
            // Sale
            services.AddScoped<ISaleCartService, SaleCartService>();
            services.AddScoped<ISaleEntryService, SaleEntryService>();
            services.AddScoped<ISalePaymentReturnService, SalePaymentReturnService>();
            services.AddScoped<ISalePaymentService, SalePaymentService>();
            services.AddScoped<ISaleReturnService, SaleReturnService>();
            services.AddScoped<ISaleService, SaleService>();
            // Miscellaneous
            services.AddScoped<IForecastingService, ForecastingService>();
            services.AddScoped<IPurchaseEntryService, PurchaseEntryService>();
            services.AddScoped<ISaleEntryService, SaleEntryService>();
            services.AddScoped<IFinancialForecaster, FinancialForecaster>();

            return services;
        }
    }
}
