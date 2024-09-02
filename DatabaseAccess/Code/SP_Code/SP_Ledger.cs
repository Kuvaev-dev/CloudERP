using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace DatabaseAccess.Code.SP_Code
{
    public class SP_Ledger
    {
        private readonly CloudDBEntities _db;

        public SP_Ledger(CloudDBEntities db)
        {
            _db = db;
        }

        public List<AccountLedgerModel> GetLedger(int CompanyID, int BranchID, int FinancialYearID)
        {
            var ledger = new List<AccountLedgerModel>();
            int sNo = 1;

            try
            {
                using (SqlConnection connection = DatabaseQuery.ConnOpen())
                {
                    using (SqlCommand command = new SqlCommand("GetLedger", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@BranchID", BranchID));
                        command.Parameters.Add(new SqlParameter("@CompanyID", CompanyID));
                        command.Parameters.Add(new SqlParameter("@FinancialYearID", FinancialYearID));

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
                                    // Add the total balance for the previous account
                                    ledger.Add(new AccountLedgerModel
                                    {
                                        SNo = sNo++,
                                        Date = Localization.Localization.TotalBalance,
                                        Debit = totalDebit.ToString(),
                                        Credit = totalCredit.ToString()
                                    });
                                }

                                // Add header for the new account
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

                            // Add the transaction details
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

                        // Add the total balance for the last account
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return ledger;
        }
    }
}