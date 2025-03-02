using Microsoft.Data.SqlClient;
using System.Data;
using Domain.RepositoryAccess;
using Domain.Models.FinancialModels;
using Utils.Helpers;
using Microsoft.Extensions.Configuration;

namespace DatabaseAccess.Repositories.Finance
{
    public class LedgerRepository : ILedgerRepository
    {
        private readonly DatabaseQuery _query;
        private readonly IConfiguration _configuration;

        public LedgerRepository(
            DatabaseQuery query,
            IConfiguration configuration)
        {
            _query = query ?? throw new ArgumentNullException(nameof(DatabaseQuery));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(IConfiguration));
        }

        public async Task<List<AccountLedgerModel>> GetLedgerAsync(int companyId, int branchId, int financialYearId)
        {
            var ledger = new List<AccountLedgerModel>();
            int sNo = 1;

            using (SqlConnection connection = await _query.ConnOpenAsync())
            {
                using SqlCommand command = new("GetLedger", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new("@BranchID", branchId));
                command.Parameters.Add(new("@CompanyID", companyId));
                command.Parameters.Add(new("@FinancialYearID", financialYearId));

                var dt = new DataTable();
                using (SqlDataAdapter da = new(command))
                {
                    da.Fill(dt);
                }

                if (dt.Rows.Count == 0)
                {
                    return ledger;
                }

                decimal totalDebit = 0;
                decimal totalCredit = 0;
                string currentAccountName = string.Empty;

                foreach (DataRow row in dt.Rows)
                {
                    var accountName = Convert.ToString(row["AccountTitle"])?.Trim() ?? string.Empty;
                    var debit = Convert.ToDecimal(row["Debit"]);
                    var credit = Convert.ToDecimal(row["Credit"]);

                    if (accountName != currentAccountName)
                    {
                        if (!string.IsNullOrEmpty(currentAccountName))
                        {
                            ledger.Add(new AccountLedgerModel
                            {
                                SNo = sNo++,
                                Date = Localization.Services.Localization.TotalBalance,
                                Debit = totalDebit.ToString(),
                                Credit = totalCredit.ToString()
                            });
                        }

                        ledger.Add(new AccountLedgerModel
                        {
                            SNo = sNo++,
                            Account = accountName,
                            Date = Localization.Services.Localization.Date,
                            Description = Localization.Services.Localization.Description,
                            Debit = Localization.Services.Localization.Debit,
                            Credit = Localization.Services.Localization.Credit
                        });

                        totalDebit = 0;
                        totalCredit = 0;
                        currentAccountName = accountName;
                    }

                    ledger.Add(new AccountLedgerModel
                    {
                        SNo = sNo++,
                        Date = Convert.ToString(row["TransectionDate"]),
                        Description = Convert.ToString(row["TransectionTitle"]),
                        Debit = debit.ToString(),
                        Credit = credit.ToString()
                    });

                    totalDebit += debit;
                    totalCredit += credit;
                }

                if (!string.IsNullOrEmpty(currentAccountName))
                {
                    ledger.Add(new AccountLedgerModel
                    {
                        SNo = sNo++,
                        Date = Localization.Services.Localization.TotalBalance,
                        Debit = totalDebit.ToString(),
                        Credit = totalCredit.ToString()
                    });
                }
            }

            return ledger;
        }
    }
}
