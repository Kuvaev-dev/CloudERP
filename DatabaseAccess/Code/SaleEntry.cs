using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DatabaseAccess.Code
{
    public class SaleEntry
    {
        private readonly CloudDBEntities _db;
        public string selectcustomerid = string.Empty;
        private DataTable _dtEntries = null;

        public SaleEntry(CloudDBEntities db)
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

        public string ConfirmSale(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, float Amount, string CustomerID, string CustomerName, bool isPayment)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    InitializeDataTable();

                    string saleTitle = "Sale to " + CustomerName.Trim();

                    // Retrieve the active financial year
                    var financialYearCheck = DatabaseQuery.Retrive("SELECT TOP 1 FinancialYearID FROM tblFinancialYear WHERE IsActive = 1");
                    string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck.Rows[0][0]) : string.Empty);
                    if (string.IsNullOrEmpty(FinancialYearID))
                    {
                        return "Your Company Financial Year is not Set! Please Contact to Administrator!";
                    }

                    // Credit Entry Sale
                    // 10 - Sale
                    var saleAccount = _db.tblAccountSetting
                        .Where(a => a.AccountActivityID == 10 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                        .FirstOrDefault();
                    if (saleAccount == null)
                    {
                        return "Account settings for Sale not found.";
                    }
                    SetEntries(FinancialYearID, saleAccount.AccountHeadID.ToString(), saleAccount.AccountControlID.ToString(), saleAccount.AccountSubControlID.ToString(), InvoiceNo, UserID.ToString(), Amount.ToString(), "0", DateTime.Now, saleTitle);

                    // Debit Entry Sale
                    // 11 - Sale Payment Pending
                    saleAccount = _db.tblAccountSetting
                        .Where(a => a.AccountActivityID == 11 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                        .FirstOrDefault();
                    if (saleAccount == null)
                    {
                        return "Account settings for Sale Payment Pending not found.";
                    }
                    SetEntries(FinancialYearID, saleAccount.AccountHeadID.ToString(), saleAccount.AccountControlID.ToString(), saleAccount.AccountSubControlID.ToString(), InvoiceNo, UserID.ToString(), "0", Amount.ToString(), DateTime.Now, CustomerName + ", Sale Payment is Pending!");

                    if (isPayment)
                    {
                        string payInvoiceNo = "INP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                        // Credit Entry Sale Payment Paid
                        // 14 - Sale Payment Paid
                        saleAccount = _db.tblAccountSetting
                            .Where(a => a.AccountActivityID == 14 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                            .FirstOrDefault();
                        if (saleAccount == null)
                        {
                            return "Account settings for Sale Payment Paid not found.";
                        }
                        SetEntries(FinancialYearID, saleAccount.AccountHeadID.ToString(), saleAccount.AccountControlID.ToString(), saleAccount.AccountSubControlID.ToString(), payInvoiceNo, UserID.ToString(), Amount.ToString(), "0", DateTime.Now, "Sale Payment Paid By " + CustomerName);

                        // Debit Entry Sale Payment Success
                        // 15 - Sale Payment Success
                        saleAccount = _db.tblAccountSetting
                            .Where(a => a.AccountActivityID == 15 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                            .FirstOrDefault();
                        if (saleAccount == null)
                        {
                            return "Account settings for Sale Payment Success not found.";
                        }
                        SetEntries(FinancialYearID, saleAccount.AccountHeadID.ToString(), saleAccount.AccountControlID.ToString(), saleAccount.AccountSubControlID.ToString(), payInvoiceNo, UserID.ToString(), "0", Amount.ToString(), DateTime.Now, CustomerName + ", Sale Payment is Succeed!");

                        // Insert payment record
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
                            new SqlParameter("@RemainingBalance", SqlDbType.Float) { Value = 0 },
                            new SqlParameter("@CompanyID", CompanyID),
                            new SqlParameter("@BranchID", BranchID),
                            new SqlParameter("@InvoiceDate", DateTime.Now.Date)
                        };

                        DatabaseQuery.Insert(paymentQuery, paymentParams);

                        return "Sale Success with Payment.";
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
                    return "Sale Success";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                    return "Unexpected Error is Occurred. Please Try Again!";
                }
            }
        }

        public string SalePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, float TotalAmount, float Amount, string CustomerID, string CustomerName, float RemainingBalance)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    InitializeDataTable();

                    string saleTitle = "Sale to " + CustomerName.Trim();

                    // Retrieve the active financial year
                    var financialYearCheck = DatabaseQuery.Retrive("SELECT TOP 1 FinancialYearID FROM tblFinancialYear WHERE IsActive = 1");
                    string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck.Rows[0][0]) : string.Empty);
                    if (string.IsNullOrEmpty(FinancialYearID))
                    {
                        return "Your Company Financial Year is not Set! Please Contact Administrator!";
                    }

                    // Credit Entry Sale Payment Paid
                    // 14 - Sale Payment Paid
                    var saleAccount = _db.tblAccountSetting
                        .Where(a => a.AccountActivityID == 14 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                        .FirstOrDefault();
                    if (saleAccount == null)
                    {
                        return "Account settings for Sale Payment Paid not found.";
                    }
                    SetEntries(FinancialYearID, saleAccount.AccountHeadID.ToString(), saleAccount.AccountControlID.ToString(), saleAccount.AccountSubControlID.ToString(), InvoiceNo, UserID.ToString(), Amount.ToString(), "0", DateTime.Now, "Sale Payment Paid By " + CustomerName);

                    // Debit Entry Sale Payment Success
                    // 15 - Sale Payment Success
                    saleAccount = _db.tblAccountSetting
                        .Where(a => a.AccountActivityID == 15 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                        .FirstOrDefault();
                    if (saleAccount == null)
                    {
                        return "Account settings for Sale Payment Success not found.";
                    }
                    SetEntries(FinancialYearID, saleAccount.AccountHeadID.ToString(), saleAccount.AccountControlID.ToString(), saleAccount.AccountSubControlID.ToString(), InvoiceNo, UserID.ToString(), "0", Amount.ToString(), DateTime.Now, CustomerName + ", Sale Payment is Succeed!");

                    // Insert payment record
                    string paymentQuery = "INSERT INTO tblCustomerPayment (CustomerID, CustomerInvoiceID, UserID, InvoiceNo, TotalAmount, PaidAmount, RemainingBalance, CompanyID, BranchID, InvoiceDate) " +
                                          "VALUES (@CustomerID, @CustomerInvoiceID, @UserID, @InvoiceNo, @TotalAmount, @PaidAmount, @RemainingBalance, @CompanyID, @BranchID, @InvoiceDate)";

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
                        new SqlParameter("@InvoiceDate", DateTime.Now.Date)
                    };

                    DatabaseQuery.Insert(paymentQuery, paymentParams);

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
                    return "Paid Successfully";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                    return "Unexpected Error is Occurred. Please Try Again!";
                }
            }
        }

        // Sale Return

        public string ReturnSale(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, int CustomerReturnInvoiceID, float Amount, string CustomerID, string Customername, bool isPayment)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    InitializeDataTable();

                    string returnSaleTitle = "Return Sale from " + Customername.Trim();

                    // Retrieve the active financial year
                    var financialYearCheck = DatabaseQuery.Retrive("SELECT TOP 1 FinancialYearID FROM tblFinancialYear WHERE IsActive = 1");
                    string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck.Rows[0][0]) : string.Empty);
                    if (string.IsNullOrEmpty(FinancialYearID))
                    {
                        return "Your Company Financial Year is not Set! Please Contact to Administrator!";
                    }

                    // Debit Entry Return Sale
                    // 12 - Sale Return
                    var returnSaleAccount = _db.tblAccountSetting
                        .Where(a => a.AccountActivityID == 12 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                        .FirstOrDefault();
                    if (returnSaleAccount == null)
                    {
                        return "Account settings for Sale Return not found.";
                    }
                    SetEntries(FinancialYearID, returnSaleAccount.AccountHeadID.ToString(), returnSaleAccount.AccountControlID.ToString(), returnSaleAccount.AccountSubControlID.ToString(), InvoiceNo, UserID.ToString(), "0", Amount.ToString(), DateTime.Now, returnSaleTitle);

                    // Credit Entry Return Sale
                    // 9 - Sale Return Payment Pending
                    returnSaleAccount = _db.tblAccountSetting
                        .Where(a => a.AccountActivityID == 9 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                        .FirstOrDefault();
                    if (returnSaleAccount == null)
                    {
                        return "Account settings for Sale Return Payment Pending not found.";
                    }
                    SetEntries(FinancialYearID, returnSaleAccount.AccountHeadID.ToString(), returnSaleAccount.AccountControlID.ToString(), returnSaleAccount.AccountSubControlID.ToString(), InvoiceNo, UserID.ToString(), Amount.ToString(), "0", DateTime.Now, Customername + ", Return Sale Payment is Pending!");

                    if (isPayment)
                    {
                        string payInvoiceNo = "RIP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                        // Credit Entry Return Sale Payment Paid
                        // 16 - Sale Return Payment Paid
                        returnSaleAccount = _db.tblAccountSetting
                            .Where(a => a.AccountActivityID == 16 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                            .FirstOrDefault();
                        if (returnSaleAccount == null)
                        {
                            return "Account settings for Sale Return Payment Pending not found.";
                        }
                        SetEntries(FinancialYearID, returnSaleAccount.AccountHeadID.ToString(), returnSaleAccount.AccountControlID.ToString(), returnSaleAccount.AccountSubControlID.ToString(), payInvoiceNo, UserID.ToString(), "0", Amount.ToString(), DateTime.Now, "Return Sale Payment Paid to " + Customername);

                        // Debit Entry Return Sale Payment Success
                        // 13 - Sale Return Payment Succeed
                        returnSaleAccount = _db.tblAccountSetting
                            .Where(a => a.AccountActivityID == 13 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                            .FirstOrDefault();
                        if (returnSaleAccount == null)
                        {
                            return "Account settings for Sale Return Payment Success not found.";
                        }
                        SetEntries(FinancialYearID, returnSaleAccount.AccountHeadID.ToString(), returnSaleAccount.AccountControlID.ToString(), returnSaleAccount.AccountSubControlID.ToString(), payInvoiceNo, UserID.ToString(), "0", Amount.ToString(), DateTime.Now, Customername + ", Return Sale Payment is Succeed!");

                        // Insert return payment record
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
                            new SqlParameter("@RemainingBalance", SqlDbType.Float) { Value = 0 },
                            new SqlParameter("@CompanyID", CompanyID),
                            new SqlParameter("@BranchID", BranchID),
                            new SqlParameter("@CustomerReturnInvoiceID", CustomerReturnInvoiceID),
                            new SqlParameter("@InvoiceDate", DateTime.Now.Date)
                        };
                        DatabaseQuery.Insert(paymentQuery, paymentParams);

                        return "Return Sale Success with Payment.";
                    }

                    // Insert transaction records
                    foreach (DataRow entryRow in _dtEntries.Rows)
                    {
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
                            new SqlParameter("@TransectionDate", DateTime.Parse(Convert.ToString(entryRow["TransectionDate"]))),
                            new SqlParameter("@TransectionTitle", Convert.ToString(entryRow["TransectionTitle"])),
                            new SqlParameter("@CompanyID", CompanyID),
                            new SqlParameter("@BranchID", BranchID)
                        };
                        DatabaseQuery.Insert(entryQuery, entryParams);
                    }

                    transaction.Commit();
                    return "Return Sale Success";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                    return "Unexpected Error is Occurred. Please Try Again!";
                }
            }
        }

        public string ReturnSalePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, int CustomerReturnInvoiceID, float TotalAmount, float Amount, string CustomerID, string Customername, float RemainingBalance)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    InitializeDataTable();

                    string saleTitle = "Return Sale from " + Customername.Trim();

                    // Retrieve the active financial year
                    var financialYearCheck = DatabaseQuery.Retrive("SELECT TOP 1 FinancialYearID FROM tblFinancialYear WHERE IsActive = 1");
                    string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck.Rows[0][0]) : string.Empty);
                    if (string.IsNullOrEmpty(FinancialYearID))
                    {
                        return "Your Company Financial Year is not Set! Please Contact to Administrator!";
                    }

                    string AccountHeadID;
                    string AccountControlID;
                    string AccountSubControlID;
                    string transactionTitle;

                    // Credit Entry Return Sale Payment Paid
                    // 16 - Sale Return Payment Paid
                    var returnSaleAccount = _db.tblAccountSetting
                        .Where(a => a.AccountActivityID == 16 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                        .FirstOrDefault();
                    if (returnSaleAccount == null)
                    {
                        return "Account settings for Sale Return Payment Pending not found.";
                    }
                    AccountHeadID = returnSaleAccount.AccountHeadID.ToString();
                    AccountControlID = returnSaleAccount.AccountControlID.ToString();
                    AccountSubControlID = returnSaleAccount.AccountSubControlID.ToString();
                    transactionTitle = "Return Sale Payment Paid to " + Customername;
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Amount.ToString(), DateTime.Now, transactionTitle);

                    // Debit Entry Return Sale Payment Success
                    // 13 - Sale Return Payment Succeed
                    returnSaleAccount = _db.tblAccountSetting
                        .Where(a => a.AccountActivityID == 13 && a.CompanyID == CompanyID && a.BranchID == BranchID)
                        .FirstOrDefault();
                    if (returnSaleAccount == null)
                    {
                        return "Account settings for Sale Return Payment Success not found.";
                    }
                    AccountHeadID = returnSaleAccount.AccountHeadID.ToString();
                    AccountControlID = returnSaleAccount.AccountControlID.ToString();
                    AccountSubControlID = returnSaleAccount.AccountSubControlID.ToString();
                    transactionTitle = Customername + ", Return Sale Payment is Succeed!";
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Amount.ToString(), "0", DateTime.Now, transactionTitle);

                    // Insert return payment record
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
                    DatabaseQuery.Insert(paymentQuery, paymentParams);

                    // Insert transaction records
                    foreach (DataRow entryRow in _dtEntries.Rows)
                    {
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
                            new SqlParameter("@TransectionDate", DateTime.Parse(Convert.ToString(entryRow["TransectionDate"]))),
                            new SqlParameter("@TransectionTitle", Convert.ToString(entryRow["TransectionTitle"])),
                            new SqlParameter("@CompanyID", CompanyID),
                            new SqlParameter("@BranchID", BranchID)
                        };
                        DatabaseQuery.Insert(entryQuery, entryParams);
                    }

                    transaction.Commit();
                    return "Paid Successfully";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                    return "Unexpected Error is Occurred. Please Try Again!";
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
