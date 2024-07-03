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
            int sNo = 0;

            var ledger = new List<AccountLedgerModel>();

            SqlCommand command = new SqlCommand("GetLedger", DatabaseQuery.ConnOpen())
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@BranchID", BranchID);
            command.Parameters.AddWithValue("@CompanyID", CompanyID);
            command.Parameters.AddWithValue("@FinancialYearID", FinancialYearID);

            var journalDB = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(journalDB);

            if (journalDB == null)
            {
                ledger.Clear();
                return ledger;
            }

            if (journalDB.Rows.Count == 0)
            {
                ledger.Clear();
                return ledger;
            }

            decimal debit = 0;
            decimal credit = 0;
            double totalRecords = 0;
            string accountName = string.Empty;

            foreach (DataRow row in journalDB.Rows)
            {
                decimal lDebit = 0;
                decimal lCredit = 0;

                if (accountName == Convert.ToString(row[7]).Trim()) // Check Account Title
                {
                    var createRow = new AccountLedgerModel();
                    createRow.SNo = sNo;
                    sNo += 1;

                    createRow.Date = Convert.ToString(row[9]); // TransectionDate
                    createRow.Description = Convert.ToString(row[10]); // TransectionTitle
                    decimal.TryParse(Convert.ToString(row[11]), out lDebit);

                    debit += lDebit;
                    createRow.Debit = Convert.ToString(row[11]);
                    decimal.TryParse(Convert.ToString(row[12]), out lCredit);

                    credit += lCredit;
                    createRow.Credit = Convert.ToString(row[12]);
                    ledger.Add(createRow);
                }
                else
                {
                    if (!string.IsNullOrEmpty(accountName))
                    {
                        var totalRow = new AccountLedgerModel();
                        totalRow.SNo = sNo;
                        sNo += 1;

                        totalRow.Date = "Total";
                        if (credit >= debit)
                        {
                            totalRow.Credit = Convert.ToString(debit - credit).Replace('-', ' ');
                        }
                        else if (credit <= debit)
                        {
                            totalRow.Debit = Convert.ToString(debit - credit).Replace('-', ' ');
                        }

                        totalRow.Date = "Total Balance";
                        ledger.Add(totalRow);
                        debit = 0;
                        credit = 0;
                    }

                    var headerRow = new AccountLedgerModel();
                    headerRow.SNo = sNo;
                    sNo += 1;
                    headerRow.Account = Convert.ToString(row[7]);
                    headerRow.Date = "Date";
                    headerRow.Description = "Description";
                    headerRow.Debit = "Debit";
                    headerRow.Credit = "Credit";
                    ledger.Add(headerRow);

                    var createRow = new AccountLedgerModel();
                    createRow.SNo = sNo;
                    sNo += 1;
                    createRow.Date = Convert.ToString(row[9]);
                    createRow.Description = Convert.ToString(row[10]);
                    decimal.TryParse(Convert.ToString(row[11]), out lCredit);
                    debit += lCredit;
                    createRow.Credit = Convert.ToString(row[12]);
                    ledger.Add(createRow);
                    accountName = Convert.ToString(row[6]).Trim();
                }
                totalRecords += 1;
                if (totalRecords == journalDB.Rows.Count)
                {
                    var totalRow = new AccountLedgerModel();
                    totalRow.SNo = sNo;
                    sNo += 1;

                    totalRow.Date = "Total";
                    if (credit >= debit)
                    {
                        totalRow.Credit = Convert.ToString(debit - credit).Replace('-', ' ');
                    }
                    else if (credit <= debit)
                    {
                        totalRow.Debit = Convert.ToString(debit - credit).Replace('-', ' ');
                    }

                    totalRow.Date = "Total Balance";
                    ledger.Add(totalRow);
                    debit = 0;
                    credit = 0;
                }
            }

            return ledger;
        }
    }
}
