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

                        decimal debit = 0;
                        decimal credit = 0;
                        double totalRecords = 0;
                        string accountName = string.Empty;

                        foreach (DataRow row in dt.Rows)
                        {
                            decimal lDebit = 0;
                            decimal lCredit = 0;

                            if (accountName == Convert.ToString(row["AccountTitle"]).Trim())
                            {
                                var createRow = new AccountLedgerModel
                                {
                                    SNo = sNo,
                                    Date = Convert.ToString(row["TransectionDate"]),
                                    Description = Convert.ToString(row["TransectionTitle"]),
                                    Debit = Convert.ToString(row["Debit"]),
                                    Credit = Convert.ToString(row["Credit"])
                                };
                                sNo += 1;

                                decimal.TryParse(Convert.ToString(row["Debit"]), out lDebit);
                                debit += lDebit;

                                decimal.TryParse(Convert.ToString(row["Credit"]), out lCredit);
                                credit += lCredit;

                                ledger.Add(createRow);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(accountName))
                                {
                                    var totalRow = new AccountLedgerModel
                                    {
                                        SNo = sNo,
                                        Date = "Total"
                                    };
                                    sNo += 1;

                                    if (credit >= debit)
                                    {
                                        totalRow.Credit = (credit - debit).ToString();
                                    }
                                    else
                                    {
                                        totalRow.Debit = (debit - credit).ToString();
                                    }

                                    totalRow.Date = "Total Balance";
                                    ledger.Add(totalRow);
                                    debit = 0;
                                    credit = 0;
                                }

                                var headerRow = new AccountLedgerModel
                                {
                                    SNo = sNo,
                                    Account = Convert.ToString(row["AccountTitle"]),
                                    Date = "Date",
                                    Description = "Description",
                                    Debit = "Debit",
                                    Credit = "Credit"
                                };
                                sNo += 1;

                                ledger.Add(headerRow);

                                var createRow = new AccountLedgerModel
                                {
                                    SNo = sNo,
                                    Date = Convert.ToString(row["TransectionDate"]),
                                    Description = Convert.ToString(row["TransectionTitle"]),
                                    Debit = Convert.ToString(row["Debit"]),
                                    Credit = Convert.ToString(row["Credit"])
                                };
                                sNo += 1;

                                decimal.TryParse(Convert.ToString(row["Debit"]), out lDebit);
                                debit += lDebit;

                                decimal.TryParse(Convert.ToString(row["Credit"]), out lCredit);
                                credit += lCredit;

                                ledger.Add(createRow);
                                accountName = Convert.ToString(row["AccountTitle"]).Trim();
                            }
                            totalRecords += 1;
                            if (totalRecords == dt.Rows.Count)
                            {
                                var totalRow = new AccountLedgerModel
                                {
                                    SNo = sNo,
                                    Date = "Total"
                                };
                                sNo += 1;

                                if (credit >= debit)
                                {
                                    totalRow.Credit = (credit - debit).ToString();
                                }
                                else
                                {
                                    totalRow.Debit = (debit - credit).ToString();
                                }

                                totalRow.Date = "Total Balance";
                                ledger.Add(totalRow);
                            }
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