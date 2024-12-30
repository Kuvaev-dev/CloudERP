using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;

namespace Domain.Services
{
    public interface IIncomeStatementService
    {
        Task<IncomeStatementModel> GetIncomeStatementAsync(int companyID, int branchID, int financialYearID);
    }

    public class IncomeStatementService : IIncomeStatementService
    {
        private readonly IBalanceSheetRepository _balanceSheetRepository;

        public IncomeStatementService(IBalanceSheetRepository balanceSheetRepository)
        {
            _balanceSheetRepository = balanceSheetRepository;
        }

        public async Task<IncomeStatementModel> GetIncomeStatementAsync(int companyID, int branchID, int financialYearID)
        {
            var incomeStatement = new IncomeStatementModel
            {
                Title = "Net Income",
                IncomeStatementHeads = new List<IncomeStatementHead>()
            };

            // Получение данных о доходах
            var revenue = await _balanceSheetRepository.GetHeadAccountsWithTotal(companyID, branchID, financialYearID, 5); // 5 - Revenue
            var revenueDetails = new IncomeStatementHead
            {
                Title = "Total Revenue",
                TotalAmount = Math.Abs(revenue.TotalAmount),
                AccountHead = revenue
            };
            incomeStatement.IncomeStatementHeads.Add(revenueDetails);

            // Получение данных о расходах
            var expenses = await _balanceSheetRepository.GetHeadAccountsWithTotal(companyID, branchID, financialYearID, 3); // 3 - Expenses
            var expensesDetails = new IncomeStatementHead
            {
                Title = "Total Expenses",
                TotalAmount = Math.Abs(expenses.TotalAmount),
                AccountHead = expenses
            };
            incomeStatement.IncomeStatementHeads.Add(expensesDetails);

            // Подсчет чистой прибыли
            incomeStatement.NetIncome = Math.Abs(revenueDetails.TotalAmount) - Math.Abs(expensesDetails.TotalAmount);

            return incomeStatement;
        }
    }
}
