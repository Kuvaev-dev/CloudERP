using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace Services.Implementations
{
    public class IncomeStatementService : IIncomeStatementService
    {
        private readonly IBalanceSheetRepository _balanceSheetRepository;

        private const int REVENUE_HEAD_ID = 6;
        private const int EXPENSES_HEAD_ID = 5;

        public IncomeStatementService(IBalanceSheetRepository balanceSheetRepository)
        {
            _balanceSheetRepository = balanceSheetRepository ?? throw new ArgumentNullException(nameof(IBalanceSheetRepository));
        }

        public async Task<IncomeStatementModel> GetIncomeStatementAsync(int companyID, int branchID, int financialYearID)
        {
            var incomeStatement = new IncomeStatementModel
            {
                Title = Localization.DatabaseAccess.Localization.NetIncome,
                IncomeStatementHeads = []
            };

            var revenue = await _balanceSheetRepository.GetHeadAccountsWithTotal(companyID, branchID, financialYearID, REVENUE_HEAD_ID);
            var revenueDetails = new IncomeStatementHead
            {
                Title = Localization.DatabaseAccess.Localization.TotalRevenue,
                TotalAmount = Math.Abs(revenue.TotalAmount),
                AccountHead = revenue
            };
            incomeStatement.IncomeStatementHeads.Add(revenueDetails);

            var expenses = await _balanceSheetRepository.GetHeadAccountsWithTotal(companyID, branchID, financialYearID, EXPENSES_HEAD_ID);
            var expensesDetails = new IncomeStatementHead
            {
                Title = Localization.DatabaseAccess.Localization.TotalExpenses,
                TotalAmount = Math.Abs(expenses.TotalAmount),
                AccountHead = expenses
            };
            incomeStatement.IncomeStatementHeads.Add(expensesDetails);

            incomeStatement.NetIncome = Math.Abs(revenueDetails.TotalAmount) - Math.Abs(expensesDetails.TotalAmount);

            return incomeStatement;
        }
    }
}
