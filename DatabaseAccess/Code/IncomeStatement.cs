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

        public List<IncomeStatementModel> GetIncomeStatement(int CompanyID, int BranchID, int FinancialYearID)
        {
            var incomeStatement = new List<IncomeStatementModel>();

            var revenue = income.GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, 5); // 5 - Revenue
            var revenueDetails = new IncomeStatementModel();
            revenueDetails.Title = "Total Revenue";
            revenueDetails.TotalAmount = revenue.TotalAmount;
            revenueDetails.AccountHeadDetails = revenue;

            var expenses = income.GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, 3); // 3 - Expenses
            var expensesDetails = new IncomeStatementModel();
            expensesDetails.Title = "Total Expenses";
            expensesDetails.TotalAmount = revenue.TotalAmount;
            expensesDetails.AccountHeadDetails = expenses;

            return incomeStatement;
        }
    }
}
