﻿using Microsoft.Data.SqlClient;
using System.Data;
using Domain.RepositoryAccess;
using Domain.Models.FinancialModels;
using Domain.UtilsAccess;

namespace DatabaseAccess.Repositories.Financial
{
    public class BalanceSheetRepository : IBalanceSheetRepository
    {
        private readonly IDatabaseQuery _query;
        private readonly IAccountHeadRepository _accountHeadRepository;

        public BalanceSheetRepository(
            IDatabaseQuery query,
            IAccountHeadRepository accountHeadRepository)
        {
            _query = query ?? throw new ArgumentNullException(nameof(query));
            _accountHeadRepository = accountHeadRepository ?? throw new ArgumentNullException(nameof(accountHeadRepository));
        }

        public async Task<double> GetAccountTotalAmountAsync(int companyId, int branchId, int financialYearId, int headId)
        {
            double totalAmount = 0;

            using (SqlConnection connection = await _query.ConnOpenAsync() as SqlConnection)
            {
                using SqlCommand command = new("GetTotalByHeadAccount", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@BranchID", branchId));
                command.Parameters.Add(new SqlParameter("@CompanyID", companyId));
                command.Parameters.Add(new SqlParameter("@HeadID", headId));
                command.Parameters.Add(new SqlParameter("@FinancialYearID", financialYearId));

                var dt = new DataTable();
                using (SqlDataAdapter da = new(command))
                {
                    da.Fill(dt);
                }

                if (dt.Rows.Count > 0)
                {
                    totalAmount = Convert.ToDouble(dt.Rows[0][0] == DBNull.Value ? 0 : dt.Rows[0][0]);
                }
            }

            return totalAmount;
        }

        public async Task<AccountHeadTotal> GetHeadAccountsWithTotal(int CompanyID, int BranchID, int FinancialYearID, int HeadID)
        {
            var accountsHeadWithDetails = new AccountHeadTotal
            {
                AccountHeadDetails = []
            };
            var accountsList = new List<AccountHeadDetail>();
            double totalAmount = 0;

            using (SqlConnection connection = await _query.ConnOpenAsync() as SqlConnection)
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
