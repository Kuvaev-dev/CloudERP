using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace DatabaseAccess.Code.SP_Code
{
    public class SP_GeneralTransaction
    {
        private readonly CloudDBEntities _db;

        public SP_GeneralTransaction(CloudDBEntities db)
        {
            _db = db;
        }

        public List<AllAccountModel> GetAllAccounts(int CompanyID, int BranchID)
        {
            var accountsList = new List<AllAccountModel>();

            try
            {
                using (SqlConnection connection = DatabaseQuery.ConnOpen())
                {
                    using (SqlCommand command = new SqlCommand("GetAllAccounts", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@BranchID", BranchID));
                        command.Parameters.Add(new SqlParameter("@CompanyID", CompanyID));

                        var dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(command))
                        {
                            da.Fill(dt);
                        }

                        foreach (DataRow row in dt.Rows)
                        {
                            var account = new AllAccountModel
                            {
                                AccountHeadID = Convert.ToInt32(row["AccountHeadID"]),
                                AccountHeadName = Convert.ToString(row["AccountHeadName"]),
                                AccountControlID = Convert.ToInt32(row["AccountControlID"]),
                                AccountControlName = Convert.ToString(row["AccountControlName"]),
                                BranchID = Convert.ToInt32(row["BranchID"]),
                                CompanyID = Convert.ToInt32(row["CompanyID"]),
                                AccountSubControlID = Convert.ToInt32(row["AccountSubControlID"]),
                                AccountSubControl = Convert.ToString(row["AccountSubControl"])
                            };

                            accountsList.Add(account);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return accountsList;
        }

        public List<JournalModel> GetJournal(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate)
        {
            var journalEntries = new List<JournalModel>();

            try
            {
                using (SqlConnection connection = DatabaseQuery.ConnOpen())
                {
                    using (SqlCommand command = new SqlCommand("GetJournal", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@BranchID", BranchID));
                        command.Parameters.Add(new SqlParameter("@CompanyID", CompanyID));
                        command.Parameters.Add(new SqlParameter("@FromDate", FromDate));
                        command.Parameters.Add(new SqlParameter("@ToDate", ToDate));

                        var dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(command))
                        {
                            da.Fill(dt);
                        }

                        int no = 1; // №
                        foreach (DataRow row in dt.Rows)
                        {
                            var entry = new JournalModel
                            {
                                TransectionDate = Convert.ToDateTime(row["TransectionDate"]),
                                AccountSubControl = Convert.ToString(row["AccountSubControl"]),
                                TransectionTitle = Convert.ToString(row["TransectionTitle"]),
                                AccountSubControlID = Convert.ToInt32(row["AccountSubControlID"]),
                                InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                                Debit = Convert.ToDouble(row["Debit"]),
                                Credit = Convert.ToDouble(row["Credit"]),
                                SNO = no
                            };
                            journalEntries.Add(entry);
                            no += 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return journalEntries;
        }
    }
}
