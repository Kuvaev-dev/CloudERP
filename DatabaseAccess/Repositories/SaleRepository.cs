using DatabaseAccess.Code;
using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface ISaleRepository
    {
        Task<List<SalePaymentModel>> RemainingPaymentList(int CompanyID, int BranchID);
        Task<List<SalePaymentModel>> CustomSalesList(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate);
        Task<List<SalePaymentModel>> SalePaymentHistory(int CustomerInvoiceID);
        Task<List<SalePaymentModel>> GetReturnSaleAmountPending(int CompanyID, int BranchID);

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

        public async Task<List<SalePaymentModel>> CustomSalesList(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate)
        {
            var remainingPaymentList = new List<SalePaymentModel>();

            try
            {
                using (SqlConnection connection = await _query.ConnOpen())
                {
                    using (SqlCommand command = new SqlCommand("GetSalesHistory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@BranchID", BranchID));
                        command.Parameters.Add(new SqlParameter("@CompanyID", CompanyID));
                        command.Parameters.Add(new SqlParameter("@FromDate", FromDate.ToString("yyyy-MM-dd")));
                        command.Parameters.Add(new SqlParameter("@ToDate", ToDate.ToString("yyyy-MM-dd")));

                        var dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(command))
                        {
                            da.Fill(dt);
                        }

                        foreach (DataRow row in dt.Rows)
                        {
                            var customerID = Convert.ToInt32(row["CustomerID"]);
                            var customer = _db.tblCustomer.Find(customerID);

                            if (customer == null)
                            {
                                Console.WriteLine($"Warning: Customer with ID {customerID} not found.");
                                continue;
                            }

                            var payment = new SalePaymentModel
                            {
                                CustomerInvoiceID = Convert.ToInt32(row["CustomerInvoiceID"]),
                                BranchID = Convert.ToInt32(row["BranchID"]),
                                CompanyID = Convert.ToInt32(row["CompanyID"]),
                                InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                                InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                                TotalAmount = Convert.ToDouble(row["BeforeReturnTotal"] == DBNull.Value ? 0 : row["BeforeReturnTotal"]),
                                ReturnProductAmount = Convert.ToDouble(row["ReturnTotal"] == DBNull.Value ? 0 : row["ReturnTotal"]),
                                AfterReturnTotalAmount = Convert.ToDouble(row["AfterReturnTotal"] == DBNull.Value ? 0 : row["AfterReturnTotal"]),
                                PaymentAmount = Convert.ToDouble(row["PaidAmount"] == DBNull.Value ? 0 : row["PaidAmount"]),
                                ReturnPaymentAmount = Convert.ToDouble(row["ReturnPayment"] == DBNull.Value ? 0 : row["ReturnPayment"]),
                                RemainingBalance = Convert.ToDouble(row["RemainingBalance"] == DBNull.Value ? 0 : row["RemainingBalance"]),
                                CustomerContactNo = customer.CustomerContact,
                                CustomerAddress = customer.CustomerAddress,
                                CustomerID = customer.CustomerID,
                                CustomerName = customer.Customername
                            };

                            remainingPaymentList.Add(payment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return remainingPaymentList;
        }

        public async Task<List<SalePaymentModel>> GetReturnSaleAmountPending(int CompanyID, int BranchID)
        {
            var remainingPaymentList = new List<SalePaymentModel>();

            try
            {
                using (SqlConnection connection = await _query.ConnOpen())
                {
                    using (SqlCommand command = new SqlCommand("GetReturnSaleAmountPending", connection))
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
                            var customerID = Convert.ToInt32(row["CustomerID"]);
                            var customer = _db.tblCustomer.Find(customerID);

                            if (customer == null)
                            {
                                Console.WriteLine($"Warning: Customer with ID {customerID} not found.");
                                continue;
                            }

                            var payment = new SalePaymentModel
                            {
                                CustomerInvoiceID = Convert.ToInt32(row["CustomerInvoiceID"]),
                                BranchID = Convert.ToInt32(row["BranchID"]),
                                CompanyID = Convert.ToInt32(row["CompanyID"]),
                                InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                                InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                                TotalAmount = Convert.ToDouble(row["BeforeReturnTotal"] == DBNull.Value ? 0 : row["BeforeReturnTotal"]),
                                ReturnProductAmount = Convert.ToDouble(row["ReturnTotal"] == DBNull.Value ? 0 : row["ReturnTotal"]),
                                AfterReturnTotalAmount = Convert.ToDouble(row["AfterReturnTotal"] == DBNull.Value ? 0 : row["AfterReturnTotal"]),
                                PaymentAmount = Convert.ToDouble(row["PaidAmount"] == DBNull.Value ? 0 : row["PaidAmount"]),
                                ReturnPaymentAmount = Convert.ToDouble(row["ReturnPayment"] == DBNull.Value ? 0 : row["ReturnPayment"]),
                                RemainingBalance = Convert.ToDouble(row["RemainingBalance"] == DBNull.Value ? 0 : row["RemainingBalance"]),
                                CustomerContactNo = customer.CustomerContact,
                                CustomerAddress = customer.CustomerAddress,
                                CustomerID = customer.CustomerID,
                                CustomerName = customer.Customername
                            };

                            remainingPaymentList.Add(payment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return remainingPaymentList;
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

        public async Task<List<SalePaymentModel>> RemainingPaymentList(int CompanyID, int BranchID)
        {
            var remainingPaymentList = new List<SalePaymentModel>();

            try
            {
                using (SqlConnection connection = await _query.ConnOpen())
                {
                    using (SqlCommand command = new SqlCommand("GetCustomerRemainingPaymentRecord", connection))
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
                            var customerID = Convert.ToInt32(row["CustomerID"]);
                            var customer = _db.tblCustomer.Find(customerID);

                            if (customer == null)
                            {
                                Console.WriteLine($"Warning: Customer with ID {customerID} not found.");
                                continue;
                            }

                            var payment = new SalePaymentModel
                            {
                                CustomerInvoiceID = Convert.ToInt32(row["CustomerInvoiceID"]),
                                BranchID = Convert.ToInt32(row["BranchID"]),
                                CompanyID = Convert.ToInt32(row["CompanyID"]),
                                InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                                InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                                PaymentAmount = Convert.ToDouble(row["Payment"] == DBNull.Value ? 0 : row["Payment"]),
                                RemainingBalance = Convert.ToDouble(row["RemainingBalance"] == DBNull.Value ? 0 : row["RemainingBalance"]),
                                CustomerContactNo = customer.CustomerContact,
                                CustomerAddress = customer.CustomerAddress,
                                CustomerID = customer.CustomerID,
                                CustomerName = customer.Customername,
                                TotalAmount = Convert.ToDouble(row["TotalAmount"] == DBNull.Value ? 0 : row["TotalAmount"])
                            };

                            remainingPaymentList.Add(payment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return remainingPaymentList;
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

        public Task<List<SalePaymentModel>> SalePaymentHistory(int CustomerInvoiceID)
        {
            throw new NotImplementedException();
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
