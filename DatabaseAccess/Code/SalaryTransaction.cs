using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DatabaseAccess;
using DatabaseAccess.Code;

public class SalaryTransaction
{
    private readonly CloudDBEntities _db;
    private DataTable _dtEntries = null;

    public SalaryTransaction(CloudDBEntities db)
    {
        _db = db;
        InitializeDataTable();
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
        using (var transaction = _db.Database.BeginTransaction())
        {
            try
            {
                InitializeDataTable();

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

                // 8 - Sale Return Payment Pending
                var account = _db.tblAccountSetting
                    .Where(a => a.tblAccountActivity.AccountActivityID == 8 && a.CompanyID == CompanyID && a.BranchID == BranchID)
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
                string paymentquery = "insert into tblPayroll(EmployeeID,BranchID,CompanyID,TransferAmount,PayrollInvoiceNo,PaymentDate,SalaryMonth,SalaryYear,UserID) " +
                                      "values(@EmployeeID,@BranchID,@CompanyID,@TransferAmount,@PayrollInvoiceNo,@PaymentDate,@SalaryMonth,@SalaryYear,@UserID)";

                SqlParameter[] paymentParameters = {
                    new SqlParameter("@EmployeeID", EmployeeID),
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@TransferAmount", TransferAmount),
                    new SqlParameter("@PayrollInvoiceNo", InvoiceNo),
                    new SqlParameter("@PaymentDate", DateTime.Parse(paymentDate)),
                    new SqlParameter("@SalaryMonth", SalaryMonth),
                    new SqlParameter("@SalaryYear", SalaryYear),
                    new SqlParameter("@UserID", UserID)
                };

                Console.WriteLine($"Executing payment query: {paymentquery}");
                DatabaseQuery.Insert(paymentquery, paymentParameters);

                foreach (DataRow entryRow in _dtEntries.Rows)
                {
                    string entryDate = Convert.ToDateTime(entryRow["TransectionDate"]).ToString("yyyy-MM-dd HH:mm:ss");
                    string entryQuery = "insert into tblTransaction(FinancialYearID,AccountHeadID,AccountControlID,AccountSubControlID,InvoiceNo,UserID,Credit,Debit,TransectionDate,TransectionTitle,CompanyID,BranchID) " +
                                        "values (@FinancialYearID,@AccountHeadID,@AccountControlID,@AccountSubControlID,@InvoiceNo,@UserID,@Credit,@Debit,@TransectionDate,@TransectionTitle,@CompanyID,@BranchID)";

                    SqlParameter[] entryParameters = {
                        new SqlParameter("@FinancialYearID", Convert.ToString(entryRow[0])),
                        new SqlParameter("@AccountHeadID", Convert.ToString(entryRow[1])),
                        new SqlParameter("@AccountControlID", Convert.ToString(entryRow[2])),
                        new SqlParameter("@AccountSubControlID", Convert.ToString(entryRow[3])),
                        new SqlParameter("@InvoiceNo", Convert.ToString(entryRow[4])),
                        new SqlParameter("@UserID", Convert.ToString(entryRow[5])),
                        new SqlParameter("@Credit", Convert.ToString(entryRow[6])),
                        new SqlParameter("@Debit", Convert.ToString(entryRow[7])),
                        new SqlParameter("@TransectionDate", DateTime.Parse(entryDate)),
                        new SqlParameter("@TransectionTitle", Convert.ToString(entryRow[9])),
                        new SqlParameter("@CompanyID", CompanyID),
                        new SqlParameter("@BranchID", BranchID)
                    };

                    Console.WriteLine($"Executing transaction entry query: {entryQuery}");
                    DatabaseQuery.Insert(entryQuery, entryParameters);
                }

                transaction.Commit();
                return "Salary Succeed";
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                return "Unexpected Error is Occured. Please Try Again!";
            }
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