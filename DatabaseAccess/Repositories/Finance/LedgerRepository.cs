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
    public class LedgerRepository : ILedgerRepository
    {
        private readonly DatabaseQuery _query;

        public LedgerRepository(DatabaseQuery query)
        {
            _query = query ?? throw new ArgumentNullException(nameof(DatabaseQuery));
        }

        public async Task<List<AccountLedgerModel>> GetLedgerAsync(int companyId, int branchId, int financialYearId)
        {
            var ledger = new List<AccountLedgerModel>();
            int sNo = 1;

            using (SqlConnection connection = await _query.ConnOpen())
            {
                using (SqlCommand command = new SqlCommand("GetLedger", connection))
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

                    if (dt.Rows.Count == 0)
                    {
                        return ledger;
                    }

                    decimal totalDebit = 0;
                    decimal totalCredit = 0;
                    string currentAccountName = string.Empty;

                    foreach (DataRow row in dt.Rows)
                    {
                        var accountName = Convert.ToString(row["AccountTitle"]).Trim();
                        var debit = Convert.ToDecimal(row["Debit"]);
                        var credit = Convert.ToDecimal(row["Credit"]);

                        if (accountName != currentAccountName)
                        {
                            if (!string.IsNullOrEmpty(currentAccountName))
                            {
                                ledger.Add(new AccountLedgerModel
                                {
                                    SNo = sNo++,
                                    Date = Localization.Localization.TotalBalance,
                                    Debit = totalDebit.ToString(),
                                    Credit = totalCredit.ToString()
                                });
                            }

                            ledger.Add(new AccountLedgerModel
                            {
                                SNo = sNo++,
                                Account = accountName,
                                Date = Localization.Localization.Date,
                                Description = Localization.Localization.Description,
                                Debit = Localization.Localization.Debit,
                                Credit = Localization.Localization.Credit
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
                            Date = Localization.Localization.TotalBalance,
                            Debit = totalDebit.ToString(),
                            Credit = totalCredit.ToString()
                        });
                    }
                }
            }

            return ledger;
        }
    }
}
