using DatabaseAccess.Code;
using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface IBalanceSheetRepository
    {
        Task<BalanceSheetModel> GetBalanceSheetAsync(int companyId, int branchId, int financialYearId, List<int> headIds);
        Task<double> GetAccountTotalAmountAsync(int companyId, int branchId, int financialYearId, int headId);
        Task<AccountHeadTotal> GetHeadAccountsWithTotal(int CompanyID, int BranchID, int FinancialYearID, int HeadID);
    }

    public class BalanceSheetRepository : IBalanceSheetRepository
    {
        private readonly CloudDBEntities _db;
        private readonly DatabaseQuery _query;

        public BalanceSheetRepository(CloudDBEntities db, DatabaseQuery query)
        {
            _db = db;
            _query = query;
        }

        public async Task<BalanceSheetModel> GetBalanceSheetAsync(int companyId, int branchId, int financialYearId, List<int> headIds)
        {
            var BalanceSheet = new BalanceSheetModel();

            double TotalAssets = 0;
            double TotalLiabilities = 0;
            double TotalOwnerEquity = 0;
            double TotalReturnEarning = 0;

            // Return Earning
            double TotalExpenses = 0;
            double TotalRevenue = 0;

            var AllHeads = new List<AccountHeadTotal>();

            foreach (var HeadID in headIds)
            {
                AccountHeadTotal AccountHead = await GetHeadAccountsWithTotal(companyId, branchId, financialYearId, HeadID);

                if (AccountHead != null && AccountHead.AccountHeadDetails != null)
                {
                    if (HeadID == 1) // Total Assets
                    {
                        TotalAssets = await GetAccountTotalAmountAsync(companyId, branchId, financialYearId, HeadID);
                    }
                    else if (HeadID == 2) // Total Liabilities
                    {
                        TotalLiabilities = await GetAccountTotalAmountAsync(companyId, branchId, financialYearId, HeadID);
                    }
                    else if (HeadID == 4) // Total Owner Equity
                    {
                        TotalOwnerEquity = await GetAccountTotalAmountAsync(companyId, branchId, financialYearId, HeadID);
                    }
                    else if (HeadID == 3) // Total Expenses
                    {
                        TotalExpenses = AccountHead.TotalAmount;
                    }
                    else if (HeadID == 5) // Total Revenue
                    {
                        TotalRevenue = AccountHead.TotalAmount;
                    }

                    AllHeads.Add(AccountHead);
                }
            }

            TotalReturnEarning = TotalRevenue - TotalExpenses;

            BalanceSheet.Title = "Total Balance";
            BalanceSheet.ReturnEarning = TotalReturnEarning;
            BalanceSheet.Total_Liabilities_OwnerEquity_ReturnEarning = TotalLiabilities + TotalOwnerEquity + TotalReturnEarning;
            BalanceSheet.TotalAssets = TotalAssets;
            BalanceSheet.AccountHeadTotals = AllHeads;

            return BalanceSheet;
        }

        public async Task<double> GetAccountTotalAmountAsync(int companyId, int branchId, int financialYearId, int headId)
        {
            double totalAmount = 0;

            try
            {
                using (SqlConnection connection = await _query.ConnOpen())
                {
                    using (SqlCommand command = new SqlCommand("GetTotalByHeadAccount", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@BranchID", branchId));
                        command.Parameters.Add(new SqlParameter("@CompanyID", companyId));
                        command.Parameters.Add(new SqlParameter("@HeadID", headId));
                        command.Parameters.Add(new SqlParameter("@FinancialYearID", financialYearId));

                        var dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(command))
                        {
                            da.Fill(dt);
                        }

                        if (dt.Rows.Count > 0)
                        {
                            totalAmount = Convert.ToDouble(dt.Rows[0][0] == DBNull.Value ? 0 : dt.Rows[0][0]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return totalAmount;
        }

        public async Task<AccountHeadTotal> GetHeadAccountsWithTotal(int CompanyID, int BranchID, int FinancialYearID, int HeadID)
        {
            var accountsHeadWithDetails = new AccountHeadTotal
            {
                AccountHeadDetails = new List<AccountHeadDetail>()
            };
            var accountsList = new List<AccountHeadDetail>();
            double totalAmount = 0;

            try
            {
                using (SqlConnection connection = await _query.ConnOpen())
                {
                    using (SqlCommand command = new SqlCommand("GetAccountHeadDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@BranchID", BranchID));
                        command.Parameters.Add(new SqlParameter("@CompanyID", CompanyID));
                        command.Parameters.Add(new SqlParameter("@HeadID", HeadID));
                        command.Parameters.Add(new SqlParameter("@FinancialYearID", FinancialYearID));

                        var dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(command))
                        {
                            da.Fill(dt);
                        }

                        foreach (DataRow row in dt.Rows)
                        {
                            var account = new AccountHeadDetail
                            {
                                AccountSubTitle = Convert.ToString(row["AccountTitle"]),
                                TotalAmount = Convert.ToDouble(row["Total"] == DBNull.Value ? 0 : row["Total"]),
                                Status = Convert.ToString(row["Status"])
                            };

                            totalAmount += account.TotalAmount;

                            if (account.TotalAmount > 0)
                            {
                                accountsList.Add(account);
                            }
                        }
                    }
                }

                var accountHead = _db.tblAccountHead.Find(HeadID);
                if (accountHead != null)
                {
                    accountsHeadWithDetails.TotalAmount = totalAmount;
                    accountsHeadWithDetails.AccountHeadTitle = accountHead.AccountHeadName;
                    accountsHeadWithDetails.AccountHeadDetails = accountsList;
                }
                else
                {
                    Console.WriteLine($"Warning: Account head with ID {HeadID} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return accountsHeadWithDetails;
        }
    }
}
