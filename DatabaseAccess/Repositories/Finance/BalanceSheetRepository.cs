using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using Domain.RepositoryAccess;
using Domain.Models.FinancialModels;
using DatabaseAccess.Helpers;

namespace DatabaseAccess.Repositories
{
    public class BalanceSheetRepository : IBalanceSheetRepository
    {
        private readonly DatabaseQuery _query;
        private readonly IAccountHeadRepository _accountHeadRepository;

        public BalanceSheetRepository(DatabaseQuery query, IAccountHeadRepository accountHeadRepository)
        {
            _query = query ?? throw new ArgumentNullException(nameof(DatabaseQuery));
            _accountHeadRepository = accountHeadRepository ?? throw new ArgumentNullException(nameof(IAccountHeadRepository));
        }

        public async Task<double> GetAccountTotalAmountAsync(int companyId, int branchId, int financialYearId, int headId)
        {
            double totalAmount = 0;

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

            var accountHead = await _accountHeadRepository.GetByIdAsync(HeadID);
            if (accountHead != null)
            {
                accountsHeadWithDetails.TotalAmount = totalAmount;
                accountsHeadWithDetails.AccountHeadTitle = accountHead.AccountHeadName;
                accountsHeadWithDetails.AccountHeadDetails = accountsList;
            }

            return accountsHeadWithDetails;
        }
    }
}
