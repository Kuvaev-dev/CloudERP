using System;
using System.Data;
using System.Linq;

namespace DatabaseAccess.Code
{
    public class SalaryTransaction
    {
        private readonly CloudDBEntities _db;
        private DataTable _dtEntries = null;

        public SalaryTransaction(CloudDBEntities db)
        {
            _db = db;
        }

        public string Confirm(
            int EmployeeID,
            double TransferAmount,
            int UserID,
            int BranchID,
            int CompanyID,
            string InvoiceNo,
            string SalaryMonth,
            string SalaryYear)
        {
            try
            {
                _dtEntries = new DataTable();
                string transectiontitle = "Salary is Pending";

                var employee = _db.tblEmployee.Find(EmployeeID);
                string employeename = string.Empty;

                if (employee != null)
                {
                    employeename = ", To " + employee.Name;
                    transectiontitle += employeename;
                }

                var financialYearCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck.Rows[0][0]) : string.Empty);
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return "Your Company Financial Year is not Set! Please Contact the Administrator!";
                }

                var account = _db.tblAccountSetting
                    .Where(a => a.tblAccountActivity.AccountActivityID == 2 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                    .FirstOrDefault();
                if (account == null)
                {
                    return "Account settings not found for the provided CompanyID and BranchID.";
                }

                string AccountHeadID = Convert.ToString(account.AccountHeadID);
                string AccountControlID = Convert.ToString(account.AccountControlID);
                string AccountSubControlID = Convert.ToString(account.AccountSubControlID);

                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(TransferAmount), DateTime.Now, transectiontitle);

                transectiontitle = "Salary Succeed " + employee.Name;

                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Convert.ToString(TransferAmount), "0", DateTime.Now, transectiontitle);

                string paymentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string paymentquery = string.Format(
                    "insert into tblPayroll(EmployeeID,BranchID,CompanyID,TransferAmount,PayrollInvoiceNo,PaymentDate,SalaryMonth,SalaryYear,UserID) " +
                    "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                    EmployeeID, BranchID, CompanyID, TransferAmount, InvoiceNo, paymentDate, SalaryMonth, SalaryYear, UserID);
                DatabaseQuery.Insert(paymentquery);

                foreach (DataRow entryRow in _dtEntries.Rows)
                {
                    string entryDate = Convert.ToDateTime(entryRow["TransectionDate"]).ToString("yyyy-MM-dd HH:mm:ss");
                    string entryQuery = string.Format(
                        "insert into tblTransaction(FinancialYearID,AccountHeadID,AccountControlID,AccountSubControlID,InvoiceNo,UserID,Credit,Debit,TransectionDate,TransectionTitle,CompanyID,BranchID) " +
                        "values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')",
                        Convert.ToString(entryRow[0]),
                        Convert.ToString(entryRow[1]),
                        Convert.ToString(entryRow[2]),
                        Convert.ToString(entryRow[3]),
                        Convert.ToString(entryRow[4]),
                        Convert.ToString(entryRow[5]),
                        Convert.ToString(entryRow[6]),
                        Convert.ToString(entryRow[7]),
                        entryDate,
                        Convert.ToString(entryRow[9]),
                        CompanyID,
                        BranchID);
                    DatabaseQuery.Insert(entryQuery);
                }

                return "Salary Succeed";
            }
            catch (Exception ex)
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