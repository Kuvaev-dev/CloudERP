using DatabaseAccess.Code;
using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DatabaseAccess.Repositories
{
    public interface IGeneralTransactionRepository
    {
        string ConfirmGeneralTransaction(
            float transferAmount,
            int userId,
            int branchId,
            int companyId,
            string invoiceNo,
            int debitAccountControlId,
            int creditAccountControlId,
            string reason);
        List<AllAccountModel> GetAllAccounts(int CompanyID, int BranchID);
        List<JournalModel> GetJournal(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate);
    }

    public class GeneralTransactionRepository : IGeneralTransactionRepository
    {
        private readonly CloudDBEntities _db;
        private DataTable _dtEntries;

        public GeneralTransactionRepository(CloudDBEntities db)
        {
            _db = db;
        }

        private void InitializeDataTable()
        {
            if (_dtEntries == null)
            {
                _dtEntries = new DataTable();
                _dtEntries.Columns.Add("FinancialYearID");
                _dtEntries.Columns.Add("AccountHeadID");
                _dtEntries.Columns.Add("AccountControlID");
                _dtEntries.Columns.Add("AccountSubControlID");
                _dtEntries.Columns.Add("InvoiceNo");
                _dtEntries.Columns.Add("UserID");
                _dtEntries.Columns.Add("Credit");
                _dtEntries.Columns.Add("Debit");
                _dtEntries.Columns.Add("TransectionDate");
                _dtEntries.Columns.Add("TransectionTitle");
            }
        }

        public string ConfirmGeneralTransaction(
            float transferAmount,
            int userId,
            int branchId,
            int companyId,
            string invoiceNo,
            int debitAccountControlId,
            int creditAccountControlId,
            string reason)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    InitializeDataTable();

                    // Заголовок транзакции
                    string transactionTitle = reason;

                    // Проверка финансового года
                    var financialYearCheck = DatabaseQuery.Retrive("SELECT TOP 1 FinancialYearID FROM tblFinancialYear WHERE IsActive = 1");
                    string financialYearId = Convert.ToString(financialYearCheck.Rows[0][0]);

                    if (string.IsNullOrEmpty(financialYearId))
                    {
                        return Localization.Localization.CompanyFinancialYearNotSet;
                    }

                    // Дебетовая запись
                    var debitAccount = _db.tblAccountSubControl.FirstOrDefault(a =>
                        a.AccountSubControlID == debitAccountControlId &&
                        a.CompanyID == companyId &&
                        a.BranchID == branchId);

                    if (debitAccount == null)
                    {
                        return Localization.Localization.DebitAccountNotFound;
                    }

                    AddEntry(financialYearId, debitAccount.AccountHeadID.ToString(),
                        debitAccount.AccountControlID.ToString(), debitAccount.AccountSubControlID.ToString(),
                        invoiceNo, userId.ToString(), "0", transferAmount.ToString(),
                        DateTime.Now, transactionTitle);

                    // Кредитовая запись
                    var creditAccount = _db.tblAccountSubControl.FirstOrDefault(a =>
                        a.AccountSubControlID == creditAccountControlId &&
                        a.CompanyID == companyId &&
                        a.BranchID == branchId);

                    if (creditAccount == null)
                    {
                        return Localization.Localization.CreditAccountNotFound;
                    }

                    transactionTitle = Localization.Localization.GeneralTransactionSucceed;

                    AddEntry(financialYearId, creditAccount.AccountHeadID.ToString(),
                        creditAccount.AccountControlID.ToString(), creditAccount.AccountSubControlID.ToString(),
                        invoiceNo, userId.ToString(), transferAmount.ToString(), "0",
                        DateTime.Now, transactionTitle);

                    // Сохранение транзакций в базу данных
                    foreach (DataRow entryRow in _dtEntries.Rows)
                    {
                        string entryDate = Convert.ToDateTime(entryRow["TransectionDate"]).ToString("yyyy-MM-dd HH:mm:ss");
                        string entryQuery = "INSERT INTO tblTransaction (FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID, Credit, Debit, TransectionDate, TransectionTitle, CompanyID, BranchID) " +
                                            "VALUES (@FinancialYearID, @AccountHeadID, @AccountControlID, @AccountSubControlID, @InvoiceNo, @UserID, @Credit, @Debit, @TransectionDate, @TransectionTitle, @CompanyID, @BranchID)";

                        var entryParams = new[]
                        {
                            new SqlParameter("@FinancialYearID", Convert.ToString(entryRow["FinancialYearID"])),
                            new SqlParameter("@AccountHeadID", Convert.ToString(entryRow["AccountHeadID"])),
                            new SqlParameter("@AccountControlID", Convert.ToString(entryRow["AccountControlID"])),
                            new SqlParameter("@AccountSubControlID", Convert.ToString(entryRow["AccountSubControlID"])),
                            new SqlParameter("@InvoiceNo", Convert.ToString(entryRow["InvoiceNo"])),
                            new SqlParameter("@UserID", Convert.ToString(entryRow["UserID"])),
                            new SqlParameter("@Credit", Convert.ToDecimal(entryRow["Credit"])),
                            new SqlParameter("@Debit", Convert.ToDecimal(entryRow["Debit"])),
                            new SqlParameter("@TransectionDate", DateTime.Parse(entryDate)),
                            new SqlParameter("@TransectionTitle", Convert.ToString(entryRow["TransectionTitle"])),
                            new SqlParameter("@CompanyID", companyId),
                            new SqlParameter("@BranchID", branchId)
                        };

                        DatabaseQuery.Insert(entryQuery, entryParams);
                    }

                    transaction.Commit();
                    return Localization.Localization.GeneralTransactionSucceed;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                    return Localization.Localization.UnexpectedErrorOccurred;
                }
            }
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
    

        private void AddEntry(string financialYearId, string accountHeadId, string accountControlId, string accountSubControlId, string invoiceNo, string userId, string credit, string debit, DateTime transactionDate, string transactionTitle)
        {
            InitializeDataTable();

            _dtEntries.Rows.Add(
                financialYearId,
                accountHeadId,
                accountControlId,
                accountSubControlId,
                invoiceNo,
                userId,
                credit,
                debit,
                transactionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                transactionTitle);
        }
    }
}
