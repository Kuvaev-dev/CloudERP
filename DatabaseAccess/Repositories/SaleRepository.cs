using DatabaseAccess.Code;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface ISaleRepository
    {
        Task<string> ConfirmSale(int CompanyID, int BranchID, int UserID, string CustomerInvoiceID, float Amount, string CustomerID, string payInvoiceNo, float RemainingBalance);
        Task<string> ReturnSale(int CompanyID, int BranchID, int UserID, string CustomerInvoiceID, int CustomerReturnInvoiceID, float Amount, string CustomerID, string payInvoiceNo, float RemainingBalance);
        Task<string> InsertTransaction(int CompanyID, int BranchID);
        Task ReturnSalePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, int CustomerReturnInvoiceID, float TotalAmount, float Amount, string CustomerID, float RemainingBalance);
    }

    public class SaleRepository : ISaleRepository
    {
        private readonly CloudDBEntities _db;
        private readonly DatabaseQuery _query;
        private DataTable _dtEntries = null;

        public SaleRepository(CloudDBEntities db, DatabaseQuery query)
        {
            _db = db;
            _query = query;
        }

        public async Task<string> ConfirmSale(int CompanyID, int BranchID, int UserID, string CustomerInvoiceID, float Amount, string CustomerID, string payInvoiceNo, float RemainingBalance)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    InitializeDataTable();

                    string paymentQuery = "INSERT INTO tblCustomerPayment (CustomerID, CustomerInvoiceID, UserID, InvoiceNo, TotalAmount, PaidAmount, RemainingBalance, CompanyID, BranchID, InvoiceDate) " +
                                                      "VALUES (@CustomerID, @CustomerInvoiceID, @UserID, @InvoiceNo, @TotalAmount, @PaidAmount, @RemainingBalance, @CompanyID, @BranchID, @InvoiceDate)";

                    var paymentParams = new[]
                    {
                        new SqlParameter("@CustomerID", CustomerID),
                        new SqlParameter("@CustomerInvoiceID", CustomerInvoiceID),
                        new SqlParameter("@UserID", UserID),
                        new SqlParameter("@InvoiceNo", payInvoiceNo),
                        new SqlParameter("@TotalAmount", Amount),
                        new SqlParameter("@PaidAmount", Amount),
                        new SqlParameter("@RemainingBalance", SqlDbType.Float) { Value = RemainingBalance != 0 ? RemainingBalance : RemainingBalance },
                        new SqlParameter("@CompanyID", CompanyID),
                        new SqlParameter("@BranchID", BranchID),
                        new SqlParameter("@InvoiceDate", DateTime.Now.Date)
                    };

                    await _query.Insert(paymentQuery, paymentParams);

                    return Localization.Localization.SaleSuccessWithPayment;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                    return Localization.Localization.UnexpectedErrorOccurred;
                }
            }
        }

        public async Task<string> InsertTransaction(int CompanyID, int BranchID)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
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

                    return Localization.Localization.PurchaseSuccess;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                    return Localization.Localization.UnexpectedErrorOccurred;
                }
            }
        }

        public async Task<string> ReturnSale(int CompanyID, int BranchID, int UserID, string CustomerInvoiceID, int CustomerReturnInvoiceID, float Amount, string CustomerID, string payInvoiceNo, float RemainingBalance)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    InitializeDataTable();
                    string paymentQuery = "INSERT INTO tblCustomerReturnPayment (CustomerID, CustomerInvoiceID, UserID, InvoiceNo, TotalAmount, PaidAmount, RemainingBalance, CompanyID, BranchID, CustomerReturnInvoiceID, InvoiceDate) " +
                    "VALUES (@CustomerID, @CustomerInvoiceID, @UserID, @InvoiceNo, @TotalAmount, @PaidAmount, @RemainingBalance, @CompanyID, @BranchID, @CustomerReturnInvoiceID, @InvoiceDate)";
                    var paymentParams = new[]
                    {
                        new SqlParameter("@CustomerID", CustomerID),
                        new SqlParameter("@CustomerInvoiceID", CustomerInvoiceID),
                        new SqlParameter("@UserID", UserID),
                        new SqlParameter("@InvoiceNo", payInvoiceNo),
                        new SqlParameter("@TotalAmount", Amount),
                        new SqlParameter("@PaidAmount", Amount),
                        new SqlParameter("@RemainingBalance", SqlDbType.Float) { Value = RemainingBalance != 0 ? RemainingBalance : RemainingBalance },
                        new SqlParameter("@CompanyID", CompanyID),
                        new SqlParameter("@BranchID", BranchID),
                        new SqlParameter("@CustomerReturnInvoiceID", CustomerReturnInvoiceID),
                        new SqlParameter("@InvoiceDate", DateTime.Now.Date)
                    };

                    await _query.Insert(paymentQuery, paymentParams);

                    return Localization.Localization.ReturnSaleSuccessWithPayment;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                    return Localization.Localization.UnexpectedErrorOccurred;
                }
            }
        }

        public async Task ReturnSalePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, int CustomerReturnInvoiceID, float TotalAmount, float Amount, string CustomerID, float RemainingBalance)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    InitializeDataTable();
                    string paymentQuery = "INSERT INTO tblCustomerReturnPayment (CustomerID, CustomerInvoiceID, UserID, InvoiceNo, TotalAmount, PaidAmount, RemainingBalance, CompanyID, BranchID, CustomerReturnInvoiceID, InvoiceDate) " +
                                          "VALUES (@CustomerID, @CustomerInvoiceID, @UserID, @InvoiceNo, @TotalAmount, @PaidAmount, @RemainingBalance, @CompanyID, @BranchID, @CustomerReturnInvoiceID, @InvoiceDate)";
                    var paymentParams = new[]
                    {
                        new SqlParameter("@CustomerID", CustomerID),
                        new SqlParameter("@CustomerInvoiceID", CustomerInvoiceID),
                        new SqlParameter("@UserID", UserID),
                        new SqlParameter("@InvoiceNo", InvoiceNo),
                        new SqlParameter("@TotalAmount", TotalAmount),
                        new SqlParameter("@PaidAmount", Amount),
                        new SqlParameter("@RemainingBalance", RemainingBalance),
                        new SqlParameter("@CompanyID", CompanyID),
                        new SqlParameter("@BranchID", BranchID),
                        new SqlParameter("@CustomerReturnInvoiceID", CustomerReturnInvoiceID),
                        new SqlParameter("@InvoiceDate", DateTime.Now.Date)
                    };
                    await _query.Insert(paymentQuery, paymentParams);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                }
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
