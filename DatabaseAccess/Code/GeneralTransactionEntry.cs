using System;
using System.Data;
using System.Linq;

namespace DatabaseAccess.Code
{
    public class GeneralTransactionEntry
    {
        private readonly CloudDBEntities _db;
        public string selectcustomerid = string.Empty;
        private DataTable dtEntries = null;

        public GeneralTransactionEntry(CloudDBEntities db)
        {
            _db = db;
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
            try
            {
                dtEntries = null;
                string transectiontitle = Reason;
                var financialYearCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck.Rows[0][0]) : string.Empty);
                
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return "Your Company Financial Year is not Set! Please Contact to Administrator!";
                }

                string AccountHeadID = string.Empty;
                string AccountControlID = string.Empty;
                string AccountSubControlID = string.Empty;

                // Assests 1      increae(Debit)   decrese(Credit)
                // Liabilities 2     increae(Credit)   decrese(Debit)
                // Expenses 3     increae(Debit)   decrese(Credit)
                // Capital 4     increae(Credit)   decrese(Debit)
                // Revenue 5     increae(Credit)   decrese(Debit)

                var account = _db.tblAccountSubControl.Where(a => a.AccountSubControlID == DebitAccountControlID && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); // 14 - Sale Return Payment Pending
                AccountHeadID = Convert.ToString(account.AccountHeadID);
                AccountControlID = Convert.ToString(account.AccountControlID);
                AccountSubControlID = Convert.ToString(account.AccountSubControlID);
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(TransferAmount), DateTime.Now, transectiontitle);

                account = _db.tblAccountSubControl.Where(a => a.AccountSubControlID == CreditAccountControlID && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); // 14 - Sale Return Payment Pending
                AccountHeadID = Convert.ToString(account.AccountHeadID);
                AccountControlID = Convert.ToString(account.AccountControlID);
                AccountSubControlID = Convert.ToString(account.AccountSubControlID);
                transectiontitle = "General Transaction Succeed!";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Convert.ToString(TransferAmount), "0", DateTime.Now, transectiontitle);

                foreach (DataRow entryRow in dtEntries.Rows)
                {
                    string entryQuery = string.Format("insert into tblTransaction(FinancialYearID,AccountHeadID,AccountControlID,AccountSubControlID,InvoiceNo,UserID,Credit,Debit,TransectionDate,TransectionTitle,CompanyID,BranchID) " +
                        "values {'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}'}",
                        Convert.ToString(entryRow[0]),
                        Convert.ToString(entryRow[1]),
                        Convert.ToString(entryRow[2]),
                        Convert.ToString(entryRow[3]),
                        Convert.ToString(entryRow[4]),
                        Convert.ToString(entryRow[5]),
                        Convert.ToString(entryRow[6]),
                        Convert.ToString(entryRow[7]),
                        Convert.ToDateTime(Convert.ToString(entryRow[8])).ToString("yyyy/MM/dd HH:mm"),
                        Convert.ToString(entryRow[9]),
                        CompanyID, BranchID);
                    DatabaseQuery.Insert(entryQuery);
                }

                return "General Transaction Succeed";
            }
            catch
            {
                return "Unexpected Error is Occured. Please Try Again!";
            }
        }

        private void SetEntries(
            string FinancialYearID,
            string AccountHeadID,
            string AccountControlID,
            string AccountSubControlID,
            string InvoiceNo,
            string UserID,
            string Credit,
            string Debit,
            DateTime TransactionDate,
            string TransectionTitle)
        {
            if (dtEntries == null)
            {
                dtEntries = new DataTable();
                dtEntries.Columns.Add("FinancialYearID");
                dtEntries.Columns.Add("AccountHeadID");
                dtEntries.Columns.Add("AccountControlID");
                dtEntries.Columns.Add("AccountSubControlID");
                dtEntries.Columns.Add("InvoiceNo");
                dtEntries.Columns.Add("UserID");
                dtEntries.Columns.Add("Credit");
                dtEntries.Columns.Add("Debit");
                dtEntries.Columns.Add("TransactionDate");
                dtEntries.Columns.Add("TransectionTitle");
            }

            if (dtEntries != null)
            {
                dtEntries.Rows.Add(
                FinancialYearID,
                AccountHeadID,
                AccountControlID,
                AccountSubControlID,
                InvoiceNo,
                UserID,
                Credit,
                Debit,
                TransactionDate.ToString("yyyy/MM/dd HH:mm"),
                TransectionTitle);
            }
        }
    }
}
