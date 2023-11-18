using DatabaseAccess.Code.SP_Code;
using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code
{
    public class IncomeStatement
    {
        private SP_BalanceSheet income = new SP_BalanceSheet();

        public IncomeStatementModel GetIncomeStatement(int CompanyID, int BranchID, int FinancialYearID)
        {
            var incomeStatement = new IncomeStatementModel();
            incomeStatement.Title = "Net Income";
            incomeStatement.IncomeStatementHeads = new List<IncomeStatementHead>();

            var revenue = income.GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, 5); // 5 - Revenue
            var revenueDetails = new IncomeStatementHead();
            revenueDetails.Title = "Total Revenue";
            revenueDetails.TotalAmount = Math.Abs(revenue.TotalAmount);
            revenueDetails.AccountHead = revenue;
            incomeStatement.IncomeStatementHeads.Add(revenueDetails);

            var expenses = income.GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, 3); // 3 - Expenses
            var expensesDetails = new IncomeStatementHead();
            expensesDetails.Title = "Total Expenses";
            expensesDetails.TotalAmount = Math.Abs(expenses.TotalAmount);
            expensesDetails.AccountHead = expenses;
            incomeStatement.IncomeStatementHeads.Add(expensesDetails);

            incomeStatement.NetIncome = Math.Abs(revenueDetails.TotalAmount) - Math.Abs(expensesDetails.TotalAmount);

            return incomeStatement;
        }
    }
}
