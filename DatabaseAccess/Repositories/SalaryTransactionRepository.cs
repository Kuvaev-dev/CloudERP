using DatabaseAccess.Helpers;
using Domain.RepositoryAccess;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class SalaryTransactionRepository : ISalaryTransactionRepository
    {
        private readonly CloudDBEntities _db;
        private readonly DatabaseQuery _query;

        private DataTable _dtEntries = null;

        public SalaryTransactionRepository(CloudDBEntities db, DatabaseQuery query)
        {
            _db = db;
            _query = query;
        }

        public async Task<string> Confirm(int EmployeeID, double TransferAmount, int UserID, int BranchID, int CompanyID, string InvoiceNo, string SalaryMonth, string SalaryYear)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                InitializeDataTable();
                string paymentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string paymentquery = "insert into tblPayroll(EmployeeID,BranchID,CompanyID,TransferAmount,PayrollInvoiceNo,PaymentDate,SalaryMonth,SalaryYear,UserID) values(@EmployeeID,@BranchID,@CompanyID,@TransferAmount,@PayrollInvoiceNo,@PaymentDate,@SalaryMonth,@SalaryYear,@UserID)";

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

                await _query.Insert(paymentquery, paymentParameters);

                transaction.Commit();
                return Localization.Localization.SalarySucceed;
            }
        }

        public async Task<string> InsertTransaction(int CompanyID, int BranchID)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                foreach (DataRow entryRow in _dtEntries.Rows)
                {
                    string entryDate = Convert.ToDateTime(entryRow["TransectionDate"]).ToString("yyyy-MM-dd HH:mm:ss");
                    string entryQuery = "INSERT INTO tblTransaction (FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID, Credit, Debit, TransectionDate, TransectionTitle, CompanyID, BranchID) VALUES (@FinancialYearID, @AccountHeadID, @AccountControlID, @AccountSubControlID, @InvoiceNo, @UserID, @Credit, @Debit, @TransectionDate, @TransectionTitle, @CompanyID, @BranchID)";

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

                    await _query.Insert(entryQuery, entryParams);
                }
                transaction.Commit();
                return Localization.Localization.SalarySucceed;
            }
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
    }
}
