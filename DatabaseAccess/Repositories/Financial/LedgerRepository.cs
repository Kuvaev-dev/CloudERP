using Microsoft.Data.SqlClient;
using System.Data;
using Domain.RepositoryAccess;
using Domain.Models.FinancialModels;
using Domain.UtilsAccess;

namespace DatabaseAccess.Repositories.Financial
{
    public class LedgerRepository : ILedgerRepository
    {
        private readonly IDatabaseQuery _query;

        public LedgerRepository(IDatabaseQuery query)
        {
            _query = query ?? throw new ArgumentNullException(nameof(query));
        }

        public async Task<List<AccountLedgerModel>> GetLedgerAsync(int companyId, int branchId, int financialYearId)
        {
            var ledger = new List<AccountLedgerModel>();
            int sNo = 1;

            using (SqlConnection connection = await _query.ConnOpenAsync() as SqlConnection)
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
                            Date = Localization.CloudERP.Modules.Miscellaneous.Miscellaneous.Date,
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
