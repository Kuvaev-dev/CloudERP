using DatabaseAccess.Code.SP_Code;
using DatabaseAccess.Models;
using System;
using System.Collections.Generic;

namespace DatabaseAccess.Code
{
    public class IncomeStatement
    {
        private readonly CloudDBEntities _db;
        private readonly SP_BalanceSheet _income;

        public IncomeStatement(CloudDBEntities db, SP_BalanceSheet income)
        {
            _db = db;
            _income = income;
        }

        public IncomeStatementModel GetIncomeStatement(int CompanyID, int BranchID, int FinancialYearID)
        {
            var incomeStatement = new IncomeStatementModel
            {
                Title = Localization.Localization.NetIncome,
                IncomeStatementHeads = new List<IncomeStatementHead>()
            };

            var revenue = _income.GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, 5); // 5 - Revenue
            var revenueDetails = new IncomeStatementHead
            {
                Title = Localization.Localization.TotalRevenue,
                TotalAmount = Math.Abs(revenue.TotalAmount),
                AccountHead = revenue
            };
            incomeStatement.IncomeStatementHeads.Add(revenueDetails);

            var expenses = _income.GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, 3); // 3 - Expenses
            var expensesDetails = new IncomeStatementHead
            {
                Title = Localization.Localization.TotalExpenses,
                TotalAmount = Math.Abs(expenses.TotalAmount),
                AccountHead = expenses
            };
            incomeStatement.IncomeStatementHeads.Add(expensesDetails);

            incomeStatement.NetIncome = Math.Abs(revenueDetails.TotalAmount) - Math.Abs(expensesDetails.TotalAmount);

            return incomeStatement;
        }
    }
}
