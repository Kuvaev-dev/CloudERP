using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using Domain.RepositoryAccess;
using Domain.Models.FinancialModels;
using Utils.Helpers;

namespace DatabaseAccess.Repositories
{
    public class TrialBalanceRepository : ITrialBalanceRepository
    {
        private readonly DatabaseQuery _query;

        public TrialBalanceRepository(DatabaseQuery query)
        {
            _query = query ?? throw new ArgumentNullException(nameof(DatabaseQuery));
        }

        public async Task<List<TrialBalanceModel>> GetTrialBalanceAsync(int branchId, int companyId, int financialYearId)
        {
            var trialBalance = new List<TrialBalanceModel>();

            using (SqlConnection connection = await _query.ConnOpen())
            {
                using (SqlCommand command = new SqlCommand("GetTrialBalance", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@BranchID", branchId));
                    command.Parameters.Add(new SqlParameter("@CompanyID", companyId));
                    command.Parameters.Add(new SqlParameter("@FinancialYearID", financialYearId));

                    var dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                    {
                        da.Fill(dt);
                    }

                    double totalDebit = 0;
                    double totalCredit = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        var balance = new TrialBalanceModel
                        {
                            FinancialYearID = Convert.ToInt32(row["FinancialYearID"]),
                            AccountSubControl = Convert.ToString(row["AccountSubControl"]),
                            AccountSubControlID = Convert.ToInt32(row["AccountSubControlID"]),
                            Debit = Convert.ToDouble(row["Debit"]),
                            Credit = Convert.ToDouble(row["Credit"]),
                            BranchID = Convert.ToInt32(row["BranchID"]),
                            CompanyID = Convert.ToInt32(row["CompanyID"])
                        };

                        totalDebit += balance.Debit;
                        totalCredit += balance.Credit;

                        if (balance.Debit > 0 || balance.Credit > 0)
                        {
                            trialBalance.Add(balance);
                        }
                    }

                    var totalBalance = new TrialBalanceModel
                    {
                        Credit = totalCredit,
                        Debit = totalDebit,
                        AccountSubControl = Localization.DatabaseAccess.Localization.Total
                    };
                    trialBalance.Add(totalBalance);
                }
            }

            return trialBalance;
        }
    }
}
