using Domain.RepositoryAccess;
using System.Data;
using Microsoft.Data.SqlClient;
using Utils.Helpers;
using Domain.Models;
using DatabaseAccess.Context;

namespace DatabaseAccess.Repositories.Sale
{
    public class SaleRepository : ISaleRepository
    {
        private readonly CloudDBEntities _dbContext;
        private readonly DatabaseQuery _query;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;

        private DataTable _dtEntries;

        public SaleRepository(
            CloudDBEntities dbContext,
            DatabaseQuery query,
            ICustomerRepository customerRepository,
            IUserRepository userRepository)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
            _query = query ?? throw new ArgumentNullException(nameof(DatabaseQuery));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(ICustomerRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(IUserRepository));
        }

        public void SetEntries(DataTable dataTable)
        {
            _dtEntries = dataTable;
        }

        public async Task<string> ConfirmSale(
            int CompanyID,
            int BranchID,
            int UserID,
            string CustomerInvoiceID,
            float Amount,
            string CustomerID,
            string payInvoiceNo,
            float RemainingBalance)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
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

                await _query.InsertAsync(paymentQuery, paymentParams);
                return Localization.Services.Localization.SaleSuccessWithPayment;
            }
        }

        public async Task<List<SaleInfo>> CustomSalesList(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate)
        {
            var remainingPaymentList = new List<SaleInfo>();

            using (SqlConnection connection = await _query.ConnOpenAsync())
            {
                using (SqlCommand command = new("GetSalesHistory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@BranchID", BranchID));
                    command.Parameters.Add(new SqlParameter("@CompanyID", CompanyID));
                    command.Parameters.Add(new SqlParameter("@FromDate", FromDate.ToString("yyyy-MM-dd")));
                    command.Parameters.Add(new SqlParameter("@ToDate", ToDate.ToString("yyyy-MM-dd")));

                    var dt = new DataTable();
                    using (SqlDataAdapter da = new(command))
                    {
                        da.Fill(dt);
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        var customerID = Convert.ToInt32(row["CustomerID"]);
                        var customer = await _customerRepository.GetByIdAsync(customerID);

                        var payment = new SaleInfo
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

            return remainingPaymentList;
        }

        public async Task<List<SaleInfo>> GetReturnSaleAmountPending(int CompanyID, int BranchID)
        {
            var remainingPaymentList = new List<SaleInfo>();

            using (SqlConnection connection = await _query.ConnOpenAsync())
            {
                using (SqlCommand command = new("GetReturnSaleAmountPending", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@BranchID", BranchID));
                    command.Parameters.Add(new SqlParameter("@CompanyID", CompanyID));

                    var dt = new DataTable();
                    using (SqlDataAdapter da = new(command))
                    {
                        da.Fill(dt);
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        var customerID = Convert.ToInt32(row["CustomerID"]);
                        var customer = await _customerRepository.GetByIdAsync(customerID);

                        var payment = new SaleInfo
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

            return remainingPaymentList;
        }

        public async Task<string> InsertTransaction(int CompanyID, int BranchID)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
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

                    await _query.InsertAsync(entryQuery, entryParams);
                }
                return Localization.Services.Localization.PurchaseSuccess;
            }
        }

        public async Task<List<SaleInfo>> RemainingPaymentList(int CompanyID, int BranchID)
        {
            var remainingPaymentList = new List<SaleInfo>();

            using (SqlConnection connection = await _query.ConnOpenAsync())
            {
                using (SqlCommand command = new("GetCustomerRemainingPaymentRecord", connection))
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
                        var customer = await _customerRepository.GetByIdAsync(customerID);

                        var payment = new SaleInfo
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

            return remainingPaymentList;
        }

        public async Task<string> ReturnSale(
            int CompanyID,
            int BranchID,
            int UserID,
            string CustomerInvoiceID,
            int CustomerReturnInvoiceID,
            float Amount,
            string CustomerID,
            string payInvoiceNo,
            float RemainingBalance)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
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

                await _query.InsertAsync(paymentQuery, paymentParams);
                return Localization.Services.Localization.ReturnSaleSuccessWithPayment;
            }
        }

        public async Task ReturnSalePayment(
            int CompanyID,
            int BranchID,
            int UserID,
            string InvoiceNo,
            string CustomerInvoiceID,
            int CustomerReturnInvoiceID,
            float TotalAmount,
            float Amount,
            string CustomerID,
            float RemainingBalance)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
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
                await _query.InsertAsync(paymentQuery, paymentParams);
            }
        }

        public async Task<List<SaleInfo>> SalePaymentHistory(int CustomerInvoiceID)
        {
            var remainingPaymentList = new List<SaleInfo>();

            using (SqlConnection connection = await _query.ConnOpenAsync())
            {
                using (SqlCommand command = new("GetCustomerPaymentHistory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@CustomerInvoiceID", CustomerInvoiceID));

                    var dt = new DataTable();
                    using (SqlDataAdapter da = new(command))
                    {
                        da.Fill(dt);
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        var customerID = Convert.ToInt32(row["CustomerID"]);
                        var userID = Convert.ToInt32(row["UserID"]);
                        var customer = await _customerRepository.GetByIdAsync(customerID);
                        var user = await _userRepository.GetByIdAsync(userID);

                        var payment = new SaleInfo
                        {
                            CustomerInvoiceID = Convert.ToInt32(row["CustomerInvoiceID"]),
                            BranchID = Convert.ToInt32(row["BranchID"]),
                            CompanyID = Convert.ToInt32(row["CompanyID"]),
                            InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                            InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                            TotalAmount = Convert.ToDouble(row["TotalAmount"] == DBNull.Value ? 0 : row["TotalAmount"]),
                            PaymentAmount = Convert.ToDouble(row["PaidAmount"] == DBNull.Value ? 0 : row["PaidAmount"]),
                            RemainingBalance = Convert.ToDouble(row["RemainingBalance"] == DBNull.Value ? 0 : row["RemainingBalance"]),
                            CustomerContactNo = customer.CustomerContact,
                            CustomerAddress = customer.CustomerAddress,
                            CustomerID = customer.CustomerID,
                            CustomerName = customer.Customername,
                            UserID = user.UserID,
                            UserName = user.UserName
                        };

                        remainingPaymentList.Add(payment);
                    }
                }
            }

            return remainingPaymentList;
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
