using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DatabaseAccess.Code
{
    public class PurchaseEntry
    {
        private CloudDBEntities _db;
        public string selectsupplierid = string.Empty;
        private DataTable _dtEntries = null;

        public PurchaseEntry(CloudDBEntities db)
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

        public string ConfirmPurchase(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, float Amount, string SupplierID, string SupplierName, bool isPayment)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    InitializeDataTable();

                    string purchaseTitle = "Purchase From " + SupplierName.Trim();

                    // Retrieve the active financial year
                    var financialYearCheck = DatabaseQuery.Retrive("SELECT TOP 1 FinancialYearID FROM tblFinancialYear WHERE IsActive = 1");
                    string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck.Rows[0][0]) : string.Empty);
                    if (string.IsNullOrEmpty(FinancialYearID))
                    {
                        return Localization.Localization.CompanyFinancialYearNotSet;
                    }

                    // Debit Entry Purchase
                    // 1 - Purchase
                    var purchaseAccount = _db.tblAccountSetting
                        .Where(a => a.AccountActivityID == 1 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                        .FirstOrDefault();
                    if (purchaseAccount == null)
                    {
                        return "Account settings for Purchase not found.";
                    }
                    SetEntries(FinancialYearID, purchaseAccount.AccountHeadID.ToString(), purchaseAccount.AccountControlID.ToString(), purchaseAccount.AccountSubControlID.ToString(), InvoiceNo, UserID.ToString(), "0", Amount.ToString(), DateTime.Now, purchaseTitle);

                    // Credit Entry Purchase Payment Pending
                    // 2 - Purchase Payment Pending
                    purchaseAccount = _db.tblAccountSetting
                        .Where(a => a.AccountActivityID == 2 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                        .FirstOrDefault();
                    if (purchaseAccount == null)
                    {
                        return "Account settings for Purchase Payment Pending not found.";
                    }
                    SetEntries(FinancialYearID, purchaseAccount.AccountHeadID.ToString(), purchaseAccount.AccountControlID.ToString(), purchaseAccount.AccountSubControlID.ToString(), InvoiceNo, UserID.ToString(), Amount.ToString(), "0", DateTime.Now, SupplierName + ", Purchase Payment is Pending!");

                    if (isPayment)
                    {
                        string payInvoiceNo = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                        // Debit Entry Purchase Payment Paid
                        // 3 - Purchase Payment Paid
                        purchaseAccount = _db.tblAccountSetting
                            .Where(a => a.AccountActivityID == 3 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                            .FirstOrDefault();
                        if (purchaseAccount == null)
                        {
                            return "Account settings for Purchase Payment Paid not found.";
                        }
                        SetEntries(FinancialYearID, purchaseAccount.AccountHeadID.ToString(), purchaseAccount.AccountControlID.ToString(), purchaseAccount.AccountSubControlID.ToString(), payInvoiceNo, UserID.ToString(), "0", Amount.ToString(), DateTime.Now, "Payment Paid to " + SupplierName);

                        // Credit Entry Purchase Payment Success
                        // 4 - Purchase Payment Success
                        purchaseAccount = _db.tblAccountSetting
                            .Where(a => a.AccountActivityID == 4 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                            .FirstOrDefault();
                        if (purchaseAccount == null)
                        {
                            return "Account settings for Purchase Payment Success not found.";
                        }
                        SetEntries(FinancialYearID, purchaseAccount.AccountHeadID.ToString(), purchaseAccount.AccountControlID.ToString(), purchaseAccount.AccountSubControlID.ToString(), payInvoiceNo, UserID.ToString(), Amount.ToString(), "0", DateTime.Now, SupplierName + ", Purchase Payment is Succeed!");

                        // Insert payment record
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
                            new SqlParameter("@RemainingBalance", SqlDbType.Float) { Value = 0 },
                            new SqlParameter("@CompanyID", CompanyID),
                            new SqlParameter("@BranchID", BranchID),
                            new SqlParameter("@InvoiceDate", DateTime.Now.Date)
                        };

                        DatabaseQuery.Insert(paymentQuery, paymentParams);

                        return "Purchase Success with Payment.";
                    }

                    // Insert transaction records
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
                    return "Purchase Success";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                    return "Unexpected Error is Occurred. Please Try Again!";
                }
            }
        }

        public string PurchasePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, float TotalAmount, float Amount, string SupplierID, string SupplierName, float RemainingBalance)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    InitializeDataTable();

                    string pruchaseTitle = "Purchase From " + SupplierName.Trim();

                    // Retrieve the active financial year
                    var financialYearCheck = DatabaseQuery.Retrive("SELECT TOP 1 FinancialYearID FROM tblFinancialYear WHERE IsActive = 1");
                    string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck.Rows[0][0]) : string.Empty);
                    if (string.IsNullOrEmpty(FinancialYearID))
                    {
                        return "Your Company Financial Year is not Set! Please Contact the Administrator!";
                    }

                    // Debit Entry Purchase
                    // 1 - Purchase
                    var purchaseAccount = _db.tblAccountSetting
                        .Where(a => a.AccountActivityID == 1 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                        .FirstOrDefault();
                    if (purchaseAccount == null)
                    {
                        return "Account settings for Purchase not found.";
                    }
                    SetEntries(FinancialYearID, purchaseAccount.AccountHeadID.ToString(), purchaseAccount.AccountControlID.ToString(), purchaseAccount.AccountSubControlID.ToString(), InvoiceNo, UserID.ToString(), "0", Amount.ToString(), DateTime.Now, pruchaseTitle);

                    // Credit Entry Purchase Payment Pending
                    // 2 - Purchase Payment Pending
                    purchaseAccount = _db.tblAccountSetting
                        .Where(a => a.AccountActivityID == 2 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                        .FirstOrDefault();
                    if (purchaseAccount == null)
                    {
                        return "Account settings for Purchase Payment Pending not found.";
                    }
                    SetEntries(FinancialYearID, purchaseAccount.AccountHeadID.ToString(), purchaseAccount.AccountControlID.ToString(), purchaseAccount.AccountSubControlID.ToString(), InvoiceNo, UserID.ToString(), Amount.ToString(), "0", DateTime.Now, SupplierName + ", Purchase Payment is Pending!");

                    // Payment Process
                    if (Amount > 0)
                    {
                        string payInvoiceNo = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                        // Credit Entry Purchase Payment Paid
                        // 3 - Purchase Payment Paid
                        purchaseAccount = _db.tblAccountSetting
                            .Where(a => a.AccountActivityID == 3 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                            .FirstOrDefault();
                        if (purchaseAccount == null)
                        {
                            return "Account settings for Purchase Payment Paid not found.";
                        }
                        SetEntries(FinancialYearID, purchaseAccount.AccountHeadID.ToString(), purchaseAccount.AccountControlID.ToString(), purchaseAccount.AccountSubControlID.ToString(), payInvoiceNo, UserID.ToString(), "0", Amount.ToString(), DateTime.Now, "Payment Paid to " + SupplierName);

                        // Debit Entry Purchase Payment Success
                        // 4 - Purchase Payment Success
                        purchaseAccount = _db.tblAccountSetting
                            .Where(a => a.AccountActivityID == 4 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                            .FirstOrDefault();
                        if (purchaseAccount == null)
                        {
                            return "Account settings for Purchase Payment Success not found.";
                        }
                        SetEntries(FinancialYearID, purchaseAccount.AccountHeadID.ToString(), purchaseAccount.AccountControlID.ToString(), purchaseAccount.AccountSubControlID.ToString(), payInvoiceNo, UserID.ToString(), Amount.ToString(), "0", DateTime.Now, SupplierName + ", Purchase Payment is Succeed!");

                        // Insert payment record
                        string paymentQuery = "INSERT INTO tblSupplierPayment (SupplierID, SupplierInvoiceID, UserID, InvoiceNo, TotalAmount, PaymentAmount, RemainingBalance, CompanyID, BranchID, InvoiceDate) " +
                                              "VALUES (@SupplierID, @SupplierInvoiceID, @UserID, @InvoiceNo, @TotalAmount, @PaymentAmount, @RemainingBalance, @CompanyID, @BranchID, @InvoiceDate)";

                        var paymentParams = new[]
                        {
                            new SqlParameter("@SupplierID", SupplierID),
                            new SqlParameter("@SupplierInvoiceID", SupplierInvoiceID),
                            new SqlParameter("@UserID", UserID),
                            new SqlParameter("@InvoiceNo", payInvoiceNo),
                            new SqlParameter("@TotalAmount", TotalAmount),
                            new SqlParameter("@PaymentAmount", Amount),
                            new SqlParameter("@RemainingBalance", RemainingBalance),
                            new SqlParameter("@CompanyID", CompanyID),
                            new SqlParameter("@BranchID", BranchID),
                            new SqlParameter("@InvoiceDate", DateTime.Now.Date)
                        };

                        DatabaseQuery.Insert(paymentQuery, paymentParams);

                        return "Purchase Success with Payment.";
                    }

                    // Insert transaction records
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
                    return "Purchase Success";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                    return "Unexpected Error Occurred. Please Try Again!";
                }
            }
        }

        // Purchase Return
        public string ReturnPurchase(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, int SupplierReturnInvoiceID, float Amount, string SupplierID, string SupplierName, bool isPayment)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    InitializeDataTable();

                    string returnPurchaseTitle = "Return Purchase to " + SupplierName.Trim();

                    // Retrieve the active financial year
                    var financialYearCheck = DatabaseQuery.Retrive("SELECT TOP 1 FinancialYearID FROM tblFinancialYear WHERE IsActive = 1");
                    string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck.Rows[0][0]) : string.Empty);
                    if (string.IsNullOrEmpty(FinancialYearID))
                    {
                        return "Your Company Financial Year is not Set! Please Contact the Administrator!";
                    }

                    string successMessage = "Return Purchase Success";

                    // Credit Entry Return Purchase
                    // 5 - Return Purchase
                    var returnPurchaseAccount = _db.tblAccountSetting
                        .Where(a => a.AccountActivityID == 5 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                        .FirstOrDefault();
                    if (returnPurchaseAccount == null)
                    {
                        return "Account settings for Return Purchase not found.";
                    }
                    SetEntries(FinancialYearID, returnPurchaseAccount.AccountHeadID.ToString(), returnPurchaseAccount.AccountControlID.ToString(), returnPurchaseAccount.AccountSubControlID.ToString(), InvoiceNo, UserID.ToString(), Amount.ToString(), "0", DateTime.Now, returnPurchaseTitle);

                    // Debit Entry Return Purchase Payment Pending
                    // 6 - Purchase Return Payment Pending
                    returnPurchaseAccount = _db.tblAccountSetting
                        .Where(a => a.AccountActivityID == 6 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                        .FirstOrDefault();
                    if (returnPurchaseAccount == null)
                    {
                        return "Account settings for Purchase Return Payment Pending not found.";
                    }
                    string pendingPaymentTitle = SupplierName + ", Return Purchase Payment is Pending!";
                    SetEntries(FinancialYearID, returnPurchaseAccount.AccountHeadID.ToString(), returnPurchaseAccount.AccountControlID.ToString(), returnPurchaseAccount.AccountSubControlID.ToString(), InvoiceNo, UserID.ToString(), "0", Amount.ToString(), DateTime.Now, pendingPaymentTitle);

                    if (isPayment)
                    {
                        string payInvoiceNo = "RPP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                        // Debit Entry Return Payment from Supplier
                        // 6 - Purchase Return Payment Pending
                        returnPurchaseAccount = _db.tblAccountSetting
                            .Where(a => a.AccountActivityID == 6 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                            .FirstOrDefault();
                        if (returnPurchaseAccount == null)
                        {
                            return "Account settings for Purchase Return Payment Pending not found.";
                        }
                        string paymentFromTitle = "Return Payment from " + SupplierName;
                        SetEntries(FinancialYearID, returnPurchaseAccount.AccountHeadID.ToString(), returnPurchaseAccount.AccountControlID.ToString(), returnPurchaseAccount.AccountSubControlID.ToString(), payInvoiceNo, UserID.ToString(), Amount.ToString(), "0", DateTime.Now, paymentFromTitle);

                        // Credit Entry Purchase Return Payment Succeed
                        // 7 - Purchase Return Payment Succeed
                        returnPurchaseAccount = _db.tblAccountSetting
                            .Where(a => a.AccountActivityID == 7 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                            .FirstOrDefault();
                        if (returnPurchaseAccount == null)
                        {
                            return "Account settings for Purchase Return Payment Succeed not found.";
                        }
                        string paymentSuccessTitle = SupplierName + ", Return Purchase Payment is Succeed!";
                        SetEntries(FinancialYearID, returnPurchaseAccount.AccountHeadID.ToString(), returnPurchaseAccount.AccountControlID.ToString(), returnPurchaseAccount.AccountSubControlID.ToString(), payInvoiceNo, UserID.ToString(), "0", Amount.ToString(), DateTime.Now, paymentSuccessTitle);

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

                        DatabaseQuery.Insert(paymentQuery, paymentParams);

                        successMessage += " with Payment.";
                    }

                    // Insert transaction records
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
                    return successMessage;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                    return "Unexpected Error Occurred. Please Try Again!";
                }
            }
        }

        public string ReturnPurchasePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, int SupplierReturnInvoiceID, float TotalAmount, float Amount, string SupplierID, string SupplierName, float RemainingBalance)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    InitializeDataTable();

                    string returnPurchaseTitle = "Return Purchase to " + SupplierName.Trim();

                    // Retrieve the active financial year
                    var financialYearCheck = DatabaseQuery.Retrive("SELECT TOP 1 FinancialYearID FROM tblFinancialYear WHERE IsActive = 1");
                    string FinancialYearID = (financialYearCheck != null && financialYearCheck.Rows.Count > 0) ? Convert.ToString(financialYearCheck.Rows[0][0]) : string.Empty;

                    if (string.IsNullOrEmpty(FinancialYearID))
                    {
                        return "Your Company Financial Year is not Set! Please Contact the Administrator!";
                    }

                    string AccountHeadID = string.Empty;
                    string AccountControlID = string.Empty;
                    string AccountSubControlID = string.Empty;
                    string transactionTitle = string.Empty;

                    // Retrieve account settings for Purchase Return Payment Pending
                    // 6 - Purchase Return Payment Pending
                    var returnPurchaseAccount = _db.tblAccountSetting
                        .Where(a => a.AccountActivityID == 6 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                        .FirstOrDefault();
                    if (returnPurchaseAccount == null)
                    {
                        return "Account settings for Purchase Return Payment Pending not found.";
                    }
                    SetEntries(FinancialYearID, returnPurchaseAccount.AccountHeadID.ToString(), returnPurchaseAccount.AccountControlID.ToString(), returnPurchaseAccount.AccountSubControlID.ToString(), InvoiceNo, UserID.ToString(), Amount.ToString(), "0", DateTime.Now, "Return Payment from " + SupplierName);

                    // Retrieve account settings for Purchase Return Payment Succeed
                    // 7 - Purchase Return Payment Succeed
                    returnPurchaseAccount = _db.tblAccountSetting
                        .Where(a => a.AccountActivityID == 7 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                        .FirstOrDefault();
                    if (returnPurchaseAccount == null)
                    {
                        return "Account settings for Purchase Return Payment Succeed not found.";
                    }
                    SetEntries(FinancialYearID, returnPurchaseAccount.AccountHeadID.ToString(), returnPurchaseAccount.AccountControlID.ToString(), returnPurchaseAccount.AccountSubControlID.ToString(), InvoiceNo, UserID.ToString(), "0", Amount.ToString(), DateTime.Now, SupplierName + ", Return Purchase Payment is Succeed!");

                    // Insert supplier return payment record
                    string paymentQuery = "INSERT INTO tblSupplierReturnPayment (SupplierID, SupplierInvoiceID, UserID, InvoiceNo, TotalAmount, PaymentAmount, RemainingBalance, CompanyID, BranchID, SupplierReturnInvoiceID, InvoiceDate) " +
                                          "VALUES (@SupplierID, @SupplierInvoiceID, @UserID, @InvoiceNo, @TotalAmount, @PaymentAmount, @RemainingBalance, @CompanyID, @BranchID, @SupplierReturnInvoiceID, @InvoiceDate)";

                    var paymentParams = new[]
                    {
                        new SqlParameter("@SupplierID", SupplierID),
                        new SqlParameter("@SupplierInvoiceID", SupplierInvoiceID),
                        new SqlParameter("@UserID", UserID),
                        new SqlParameter("@InvoiceNo", InvoiceNo),
                        new SqlParameter("@TotalAmount", TotalAmount),
                        new SqlParameter("@PaymentAmount", Amount),
                        new SqlParameter("@RemainingBalance", RemainingBalance),
                        new SqlParameter("@CompanyID", CompanyID),
                        new SqlParameter("@BranchID", BranchID),
                        new SqlParameter("@SupplierReturnInvoiceID", SupplierReturnInvoiceID),
                        new SqlParameter("@InvoiceDate", DateTime.Now.Date)
                    };

                    DatabaseQuery.Insert(paymentQuery, paymentParams);

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
                    return "Return Purchase Payment is Paid";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                    return "Unexpected Error Occurred. Please Try Again!";
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
