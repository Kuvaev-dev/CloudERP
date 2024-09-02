using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DatabaseAccess.Code
{
    public class GeneralTransactionEntry
    {
        private readonly CloudDBEntities _db;
        public string selectcustomerid = string.Empty;
        private DataTable _dtEntries = null;

        public GeneralTransactionEntry(CloudDBEntities db)
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
            float TransferAmount,
            int UserID,
            int BranchID,
            int CompanyID,
            string InvoiceNo,
            int DebitAccountControlID,
            int CreditAccountControlID,
            string Reason)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    InitializeDataTable();

                    string transectiontitle = Reason;

                    // Retrieve the active financial year
                    var financialYearCheck = DatabaseQuery.Retrive("SELECT TOP 1 FinancialYearID FROM tblFinancialYear WHERE IsActive = 1");
                    string FinancialYearID = Convert.ToString(financialYearCheck.Rows[0][0]);

                    if (string.IsNullOrEmpty(FinancialYearID))
                    {
                        return Localization.Localization.CompanyFinancialYearNotSet;
                    }

                    // Debit entry
                    var debitAccount = _db.tblAccountSubControl.FirstOrDefault(a => a.AccountSubControlID == DebitAccountControlID && a.CompanyID == CompanyID && a.BranchID == BranchID);
                    if (debitAccount == null)
                    {
                        return Localization.Localization.DebitAccountNotFound;
                    }
                    SetEntries(FinancialYearID, debitAccount.AccountHeadID.ToString(), debitAccount.AccountControlID.ToString(), debitAccount.AccountSubControlID.ToString(), InvoiceNo, UserID.ToString(), "0", TransferAmount.ToString(), DateTime.Now, transectiontitle);

                    // Credit entry
                    var creditAccount = _db.tblAccountSubControl.FirstOrDefault(a => a.AccountSubControlID == CreditAccountControlID && a.CompanyID == CompanyID && a.BranchID == BranchID);
                    if (creditAccount == null)
                    {
                        return Localization.Localization.CreditAccountNotFound;
                    }
                    transectiontitle = Localization.Localization.GeneralTransactionSucceed;
                    SetEntries(FinancialYearID, creditAccount.AccountHeadID.ToString(), creditAccount.AccountControlID.ToString(), creditAccount.AccountSubControlID.ToString(), InvoiceNo, UserID.ToString(), TransferAmount.ToString(), "0", DateTime.Now, transectiontitle);

                    // Insert transaction entries
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
                            new SqlParameter("@CompanyID", CompanyID),
                            new SqlParameter("@BranchID", BranchID)
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

        private void SetEntries(string FinancialYearID, string AccountHeadID, string AccountControlID, string AccountSubControlID, string InvoiceNo, string UserID, string Credit, string Debit, DateTime TransactionDate, string TransectionTitle)
        {
            InitializeDataTable();

            int columnCount = _dtEntries.Columns.Count;
            int itemCount = new object[]
            {
                FinancialYearID,
                AccountHeadID,
                AccountControlID,
                AccountSubControlID,
                InvoiceNo,
                UserID,
                Credit,
                Debit,
                TransactionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                TransectionTitle
            }.Length;

            if (itemCount == columnCount)
            {
                _dtEntries.Rows.Add(
                    FinancialYearID,
                    AccountHeadID,
                    AccountControlID,
                    AccountSubControlID,
                    InvoiceNo,
                    UserID,
                    Credit,
                    Debit,
                    TransactionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    TransectionTitle);
            }
        }
    }
}
