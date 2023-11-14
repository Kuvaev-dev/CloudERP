using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code.SP_Code
{
    public class SP_BalanceSheet
    {
        /*  Account Heads by ID:
         *  1. Assets
         *  2. Liabilities
         *  3. Expenses
         *  4. Capital
         *  5. Revenue
         */

        private CloudDBEntities db = new CloudDBEntities();

        public BalanceSheetModel GetBalanceSheet(int CompanyID, int BranchID, int FinancialYearID, List<int> HeadIDs)
        {
            var BalanceSheet = new BalanceSheetModel();
            
            double TotalAssests = 0;
            double TotalLiabilities = 0;
            double TotalOwnerEquity = 0;
            double TotalReturnEarning = 0;

            // Return Earning
            double TotalExpenses = 0;
            double TotalRevenue = 0;

            var AllHeads = new List<AccountHeadTotal>();

            foreach (var HeadID in HeadIDs)
            {
                var AccountHead = new AccountHeadTotal();
                if (HeadID == 1 || HeadID == 2 || HeadID == 4)
                {
                    AccountHead = GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, HeadID);
                    if (HeadID == 1) // Total Assets
                    {
                        TotalAssests = AccountHead.TotalAccount;
                    }
                    else if (HeadID == 2) // Total Liabilities
                    {
                        TotalLiabilities = AccountHead.TotalAccount;
                    }
                    else if (HeadID == 4) // Total Owner Equity
                    {
                        TotalOwnerEquity = AccountHead.TotalAccount;
                    }

                    AllHeads.Add(AccountHead);
                }
                else if (HeadID == 3) // Total Expenses
                {
                    AccountHead = GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, HeadID);
                    TotalExpenses = AccountHead.TotalAccount;
                }
                else if (HeadID == 5) // Total Revenue
                {
                    AccountHead = GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, HeadID);
                    TotalRevenue = AccountHead.TotalAccount;
                }
            }

            TotalReturnEarning = TotalRevenue - TotalExpenses;

            BalanceSheet.Title = "Total Balance";
            BalanceSheet.ReturnEarning = TotalReturnEarning;
            BalanceSheet.Total_Liabilities_OwnerEquity_ReturnEarning = TotalLiabilities + TotalOwnerEquity + TotalReturnEarning;
            BalanceSheet.TotalAssests = TotalAssests;
            BalanceSheet.AccountHeadTotals = AllHeads;

            return BalanceSheet;
        }

        public AccountHeadTotal GetHeadAccountsWithTotal(int CompanyID, int BranchID, int FinancialYearID, int HeadID)
        {
            var accountsHeadWithDetails = new AccountHeadTotal();
            SqlCommand command = new SqlCommand("GetAccountHeadDetails", DatabaseQuery.ConnOpen())
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@BranchID", BranchID);
            command.Parameters.AddWithValue("@CompanyID", CompanyID);
            command.Parameters.AddWithValue("@HeadID", HeadID);
            command.Parameters.AddWithValue("@FinancialYearID", FinancialYearID);
            var dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dt);
            var accountsList = new List<AccountHeadDetail>();
            double totalAmount = 0;
            foreach (DataRow row in dt.Rows)
            {
                var account = new AccountHeadDetail();
                account.AccountSubTitle = Convert.ToString(row[0].ToString());
                account.TotalAmount = Convert.ToDouble(row[1]);
                totalAmount += account.TotalAmount;
                accountsList.Add(account);
            }
            var accountHead = db.tblAccountHead.Find(HeadID);
            accountsHeadWithDetails.AccountHeadTitle = accountHead.AccountHeadName;
            accountsHeadWithDetails.AccountHeadDetails = accountsList;

            return accountsHeadWithDetails;
        }
    }
}
