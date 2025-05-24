using Microsoft.Data.SqlClient;
using System.Data;
using Domain.RepositoryAccess;
using Domain.Models.FinancialModels;
using Microsoft.Extensions.Configuration;
using Domain.UtilsAccess;

namespace DatabaseAccess.Repositories.Financial
{
    public class TrialBalanceRepository : ITrialBalanceRepository
    {
        private readonly IDatabaseQuery _query;
        private readonly IConfiguration _configuration;

        public TrialBalanceRepository(
            IDatabaseQuery query,
            IConfiguration configuration)
        {
            _query = query ?? throw new ArgumentNullException(nameof(query));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<List<TrialBalanceModel>> GetTrialBalanceAsync(int branchId, int companyId, int financialYearId)
        {
            var trialBalance = new List<TrialBalanceModel>();

            using (SqlConnection connection = await _query.ConnOpenAsync() as SqlConnection)
            {
                using (SqlCommand command = new("GetTrialBalance", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new("@BranchID", branchId));
                    command.Parameters.Add(new("@CompanyID", companyId));
                    command.Parameters.Add(new("@FinancialYearID", financialYearId));

                    var dt = new DataTable();
                    using (SqlDataAdapter da = new(command))
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
                        AccountSubControl = Localization.CloudERP.Modules.Financial.Financial.Total
                    };
                    trialBalance.Add(totalBalance);
                }
            }

            return trialBalance;
        }
    }
}
