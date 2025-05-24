using Microsoft.Data.SqlClient;
using System.Data;
using Domain.RepositoryAccess;
using Domain.Models.FinancialModels;
using Domain.Models;
using DatabaseAccess.Context;
using Domain.UtilsAccess;

namespace DatabaseAccess.Repositories.Purchase
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly CloudDBEntities _dbContext;
        private readonly IDatabaseQuery _query;
        private readonly ISupplierRepository _suppliers;
        private readonly IUserRepository _users;

        private DataTable _dtEntries;

        public PurchaseRepository(
            CloudDBEntities dbContext,
            IDatabaseQuery query,
            ISupplierRepository suppliers,
            IUserRepository users)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _query = query ?? throw new ArgumentNullException(nameof(query));
            _suppliers = suppliers ?? throw new ArgumentNullException(nameof(suppliers));
            _users = users ?? throw new ArgumentNullException(nameof(users));
        }

        public async Task SetEntries(DataTable dataTable)
        {
            _dtEntries = dataTable;
        }

        public async Task<List<PurchaseInfo>> RemainingPaymentList(int CompanyID, int BranchID)
        {
            var remainingPaymentList = new List<PurchaseInfo>();

            using (SqlCommand command = new("GetSupplierRemainingPaymentRecord", await _query.ConnOpenAsync() as SqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@BranchID", BranchID);
                command.Parameters.AddWithValue("@CompanyID", CompanyID);

                var dt = new DataTable();
                using (SqlDataAdapter da = new(command))
                {
                    da.Fill(dt);
                }

                foreach (DataRow row in dt.Rows)
                {
                    var supplierID = Convert.ToInt32(row["SupplierID"]);
                    var supplier = await _suppliers.GetByIdAsync(supplierID);

                    var payment = new PurchaseInfo
                    {
                        SupplierInvoiceID = Convert.ToInt32(row["SupplierInvoiceID"]),
                        BranchID = Convert.ToInt32(row["BranchID"]),
                        CompanyID = Convert.ToInt32(row["CompanyID"]),
                        InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                        InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                        TotalAmount = Convert.ToDouble(row["BeforeReturnTotal"]),
                        ReturnProductAmount = Convert.ToDouble(row["ReturnTotal"]),
                        PaymentAmount = Convert.ToDouble(row["PaidAmount"]),
                        ReturnPaymentAmount = Convert.ToDouble(row["ReturnPayment"]),
                        RemainingBalance = Convert.ToDouble(row["RemainingBalance"]),
                        SupplierContactNo = supplier.SupplierConatctNo,
                        SupplierAddress = supplier.SupplierAddress,
                        SupplierID = supplier.SupplierID,
                        SupplierName = supplier.SupplierName
                    };

                    remainingPaymentList.Add(payment);
                }
            }

            return remainingPaymentList;
        }

        public async Task<List<PurchaseInfo>> CustomPurchasesList(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate)
        {
            var remainingPaymentList = new List<PurchaseInfo>();

            using (SqlCommand command = new("GetPurchasesHistory", await _query.ConnOpenAsync() as SqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@BranchID", BranchID);
                command.Parameters.AddWithValue("@CompanyID", CompanyID);
                command.Parameters.AddWithValue("@FromDate", FromDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@ToDate", ToDate.ToString("yyyy-MM-dd"));

                var dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(command))
                {
                    da.Fill(dt);
                }

                foreach (DataRow row in dt.Rows)
                {
                    var supplierID = Convert.ToInt32(row["SupplierID"]);
                    var supplier = await _suppliers.GetByIdAsync(supplierID);

                    var payment = new PurchaseInfo
                    {
                        SupplierInvoiceID = Convert.ToInt32(row["SupplierInvoiceID"]),
                        BranchID = Convert.ToInt32(row["BranchID"]),
                        CompanyID = Convert.ToInt32(row["CompanyID"]),
                        InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                        InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                        TotalAmount = Convert.ToDouble(row["BeforeReturnTotal"]),
                        ReturnProductAmount = Convert.ToDouble(row["ReturnTotal"]),
                        PaymentAmount = Convert.ToDouble(row["PaidAmount"]),
                        ReturnPaymentAmount = Convert.ToDouble(row["ReturnPayment"]),
                        RemainingBalance = Convert.ToDouble(row["RemainingBalance"]),
                        SupplierContactNo = supplier.SupplierConatctNo,
                        SupplierAddress = supplier.SupplierAddress,
                        SupplierID = supplier.SupplierID,
                        SupplierName = supplier.SupplierName
                    };

                    remainingPaymentList.Add(payment);
                }
            }

            return remainingPaymentList;
        }

        public async Task<List<PurchaseInfo>> PurchasePaymentHistory(int SupplierInvoiceID)
        {
            var remainingPaymentList = new List<PurchaseInfo>();

            using (SqlCommand command = new("GetSupplierPaymentHistory", await _query.ConnOpenAsync() as SqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SupplierInvoiceID", SupplierInvoiceID);

                var dt = new DataTable();
                using (SqlDataAdapter da = new(command))
                {
                    da.Fill(dt);
                }

                foreach (DataRow row in dt.Rows)
                {
                    var supplier = await _suppliers.GetByIdAsync(Convert.ToInt32(row["SupplierID"]));
                    var user = await _users.GetByIdAsync(Convert.ToInt32(row["UserID"]));

                    var payment = new PurchaseInfo
                    {
                        SupplierInvoiceID = Convert.ToInt32(row["SupplierInvoiceID"]),
                        BranchID = Convert.ToInt32(row["BranchID"]),
                        CompanyID = Convert.ToInt32(row["CompanyID"]),
                        InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                        InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                        TotalAmount = Convert.ToDouble(row["TotalAmount"]),
                        PaymentAmount = Convert.ToDouble(row["PaymentAmount"]),
                        RemainingBalance = Convert.ToDouble(row["RemainingBalance"]),
                        SupplierContactNo = supplier.SupplierConatctNo,
                        SupplierAddress = supplier.SupplierAddress,
                        SupplierID = supplier.SupplierID,
                        SupplierName = supplier.SupplierName,
                        UserID = user.UserID,
                        UserName = user.UserName
                    };

                    remainingPaymentList.Add(payment);
                }
            }

            return remainingPaymentList;
        }

        public async Task<List<SupplierReturnInvoiceModel>> PurchaseReturnPaymentPending(int? SupplierInvoiceID)
        {
            var remainingPaymentList = new List<SupplierReturnInvoiceModel>();

            using (SqlCommand command = new("GetSupplierReturnPurchasePaymentPending", await _query.ConnOpenAsync() as SqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SupplierInvoiceID", SupplierInvoiceID.HasValue ? SupplierInvoiceID.Value : DBNull.Value);

                var dt = new DataTable();
                using (SqlDataAdapter da = new(command))
                {
                    da.Fill(dt);
                }

                foreach (DataRow row in dt.Rows)
                {
                    var supplier = await _suppliers.GetByIdAsync(Convert.ToInt32(row["SupplierID"]));
                    var user = await _users.GetByIdAsync(Convert.ToInt32(row["UserID"]));

                    var payment = new SupplierReturnInvoiceModel
                    {
                        SupplierReturnInvoiceID = Convert.ToInt32(row["SupplierReturnInvoiceID"]),
                        SupplierInvoiceID = Convert.ToInt32(row["SupplierInvoiceID"]),
                        BranchID = Convert.ToInt32(row["BranchID"]),
                        CompanyID = Convert.ToInt32(row["CompanyID"]),
                        InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                        InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                        ReturnTotalAmount = Convert.ToDouble(row["ReturnTotal"]),
                        ReturnPaymentAmount = Convert.ToDouble(row["ReturnPayment"]),
                        RemainingBalance = Convert.ToDouble(row["ReturnRemainingPayment"]),
                        SupplierContactNo = supplier.SupplierConatctNo,
                        SupplierAddress = supplier.SupplierAddress,
                        SupplierID = supplier.SupplierID,
                        SupplierName = supplier.SupplierName,
                        UserID = user.UserID,
                        UserName = user.UserName
                    };

                    remainingPaymentList.Add(payment);
                }
            }

            return remainingPaymentList;
        }

        public async Task<List<PurchaseInfo>> GetReturnPurchasesPaymentPending(int CompanyID, int BranchID)
        {
            var remainingPaymentList = new List<PurchaseInfo>();

            using (SqlCommand command = new("GetReturnPurchasePaymentPending", await _query.ConnOpenAsync() as SqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@BranchID", BranchID);
                command.Parameters.AddWithValue("@CompanyID", CompanyID);

                var dt = new DataTable();
                using (SqlDataAdapter da = new(command))
                {
                    da.Fill(dt);
                }

                foreach (DataRow row in dt.Rows)
                {
                    var supplier = await _suppliers.GetByIdAsync(Convert.ToInt32(row["SupplierID"]));

                    var payment = new PurchaseInfo
                    {
                        SupplierInvoiceID = Convert.ToInt32(row["SupplierInvoiceID"]),
                        BranchID = Convert.ToInt32(row["BranchID"]),
                        CompanyID = Convert.ToInt32(row["CompanyID"]),
                        InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                        InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                        TotalAmount = Convert.ToDouble(row["BeforeReturnTotal"]),
                        ReturnProductAmount = Convert.ToDouble(row["ReturnTotal"]),
                        PaymentAmount = Convert.ToDouble(row["PaidAmount"]),
                        ReturnPaymentAmount = Convert.ToDouble(row["ReturnPayment"]),
                        RemainingBalance = Convert.ToDouble(row["RemainingBalance"]),
                        SupplierContactNo = supplier.SupplierConatctNo,
                        SupplierAddress = supplier.SupplierAddress,
                        SupplierID = supplier.SupplierID,
                        SupplierName = supplier.SupplierName
                    };

                    remainingPaymentList.Add(payment);
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

        public async Task<string> ConfirmPurchase(int CompanyID, int BranchID, int UserID, string SupplierInvoiceID, float Amount, string SupplierID, string payInvoiceNo, float RemainingBalance)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                InitializeDataTable();

                string paymentQuery = "INSERT INTO tblSupplierPayment (SupplierID, SupplierInvoiceID, UserID, InvoiceNo, TotalAmount, PaymentAmount, RemainingBalance, CompanyID, BranchID, InvoiceDate) " +
                                            "VALUES (@SupplierID, @SupplierInvoiceID, @UserID, @InvoiceNo, @TotalAmount, @PaymentAmount, @RemainingBalance, @CompanyID, @BranchID, @InvoiceDate)";

                var paymentParams = new[]
                {
                    new SqlParameter("@SupplierID", SupplierID),
                    new SqlParameter("@SupplierInvoiceID", SupplierInvoiceID),
                    new SqlParameter("@UserID", UserID),
                    new SqlParameter("@InvoiceNo", payInvoiceNo),
                    new SqlParameter("@TotalAmount", Amount),
                    new SqlParameter("@PaymentAmount", Amount),
                    new SqlParameter("@RemainingBalance", SqlDbType.Float) { Value = RemainingBalance != 0 ? RemainingBalance : RemainingBalance },
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@InvoiceDate", DateTime.Now.Date)
                };

                await _query.ExecuteNonQueryAsync(paymentQuery, paymentParams);

                await InsertTransaction(CompanyID, BranchID);

                return Localization.Services.Localization.PurchaseSuccessWithPayment;
            }
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

                    await _query.ExecuteNonQueryAsync(entryQuery, entryParams);
                }
                return Localization.Services.Localization.PurchaseSuccess;
            }
        }

        public async Task<string> ConfirmPurchaseReturn(int CompanyID, int BranchID, int UserID, string SupplierInvoiceID, int SupplierReturnInvoiceID, float Amount, string SupplierID, string payInvoiceNo)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                string paymentQuery = "INSERT INTO tblSupplierReturnPayment (SupplierID, SupplierInvoiceID, UserID, InvoiceNo, TotalAmount, PaymentAmount, RemainingBalance, CompanyID, BranchID, SupplierReturnInvoiceID, InvoiceDate) " +
                                            "VALUES (@SupplierID, @SupplierInvoiceID, @UserID, @InvoiceNo, @TotalAmount, @PaymentAmount, @RemainingBalance, @CompanyID, @BranchID, @SupplierReturnInvoiceID, @InvoiceDate)";

                var paymentParams = new[]
                {
                    new SqlParameter("@SupplierID", SupplierID),
                    new SqlParameter("@SupplierInvoiceID", SupplierInvoiceID),
                    new SqlParameter("@UserID", UserID),
                    new SqlParameter("@InvoiceNo", payInvoiceNo),
                    new SqlParameter("@TotalAmount", Amount),
                    new SqlParameter("@PaymentAmount", Amount),
                    new SqlParameter("@RemainingBalance", SqlDbType.Float) { Value = 0 },
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@SupplierReturnInvoiceID", SupplierReturnInvoiceID),
                    new SqlParameter("@InvoiceDate", DateTime.Now.Date)
                };

                await _query.ExecuteNonQueryAsync(paymentQuery, paymentParams);
                return Localization.Services.Localization.WithPayment;
            }
        }
    }
}