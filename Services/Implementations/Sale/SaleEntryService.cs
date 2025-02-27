using Domain.Facades;
using Domain.Models;
using Domain.ServiceAccess;
using System.Data;

namespace Services.Implementations
{
    public class SaleEntryService : ISaleEntryService
    {
        private readonly SaleEntryFacade _saleEntryFacade;
        private readonly string selectcustomerid = string.Empty;
        private DataTable _dtEntries;

        private const int SALE_ACCOUNT_ACTIVITY_ID = 9;
        private const int SALE_PAYMENT_PENDING_ACTIVITY_ID = 10;
        private const int SALE_PAYMENT_PAID_ACTIVITY_ID = 11;
        private const int SALE_PAYMENT_SUCCESS_ACTIVITY_ID = 12;
        private const int SALE_RETURN_ACTIVITY_ID = 13;
        private const int SALE_RETURN_PAYMENT_PENDING_ACTIVITY_ID = 8;
        private const int SALE_RETURN_PAYMENT_PAID_ACTIVITY_ID = 14;
        private const int SALE_RETURN_PAYMENT_SUCCESS_ACTIVITY_ID = 15;

        public SaleEntryService(SaleEntryFacade saleEntryFacade)
        {
            _saleEntryFacade = saleEntryFacade ?? throw new ArgumentNullException(nameof(SaleEntryFacade));
        }

        public async Task<string> ConfirmSale(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, float Amount, string CustomerID, string CustomerName, bool isPayment)
        {
            try
            {
                _dtEntries = new DataTable();
                string saleTitle = Localization.DatabaseAccess.Localization.SaleTo + CustomerName.Trim();

                // Retrieve the active financial year
                var financialYearCheck = await _saleEntryFacade.FinancialYearRepository.GetSingleActiveAsync();
                string FinancialYearID = financialYearCheck != null ? Convert.ToString(financialYearCheck) : string.Empty;
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return Localization.DatabaseAccess.Localization.CompanyFinancialYearNotSet;
                }

                // Credit Entry Sale
                var saleAccount = await _saleEntryFacade.AccountSettingRepository.GetByActivityAsync(SALE_ACCOUNT_ACTIVITY_ID, CompanyID, BranchID);
                if (saleAccount == null)
                {
                    return Localization.DatabaseAccess.Localization.AccountSettingsForSaleNotFound;
                }
                SetEntries(FinancialYearID,
                    saleAccount.AccountHeadID.ToString(),
                    saleAccount.AccountControlID.ToString(),
                    saleAccount.AccountSubControlID.ToString(),
                    InvoiceNo,
                    UserID.ToString(),
                    Amount.ToString(),
                    "0",
                    DateTime.Now,
                    saleTitle);

                // Debit Entry Sale
                saleAccount = await _saleEntryFacade.AccountSettingRepository.GetByActivityAsync(SALE_PAYMENT_PENDING_ACTIVITY_ID, CompanyID, BranchID);
                if (saleAccount == null)
                {
                    return Localization.DatabaseAccess.Localization.AccountSettingsForSalePaymentPendingNotFound;
                }
                SetEntries(FinancialYearID,
                    saleAccount.AccountHeadID.ToString(),
                    saleAccount.AccountControlID.ToString(),
                    saleAccount.AccountSubControlID.ToString(),
                    InvoiceNo,
                    UserID.ToString(),
                    "0",
                    Amount.ToString(),
                    DateTime.Now,
                    CustomerName + $", Sale Payment Is Pending");

                if (isPayment)
                {
                    string payInvoiceNo = "INP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                    // Credit Entry Sale Payment Paid
                    saleAccount = await _saleEntryFacade.AccountSettingRepository.GetByActivityAsync(SALE_PAYMENT_PAID_ACTIVITY_ID, CompanyID, BranchID);
                    if (saleAccount == null)
                    {
                        return Localization.DatabaseAccess.Localization.AccountSettingsForSalePaymentPaidNotFound;
                    }
                    SetEntries(FinancialYearID,
                        saleAccount.AccountHeadID.ToString(),
                        saleAccount.AccountControlID.ToString(),
                        saleAccount.AccountSubControlID.ToString(),
                        payInvoiceNo,
                        UserID.ToString(),
                        Amount.ToString(),
                        "0",
                        DateTime.Now,
                        $"Sale Payment Paid By " + CustomerName);

                    // Debit Entry Sale Payment Success
                    saleAccount = await _saleEntryFacade.AccountSettingRepository.GetByActivityAsync(SALE_PAYMENT_SUCCESS_ACTIVITY_ID, CompanyID, BranchID);
                    if (saleAccount == null)
                    {
                        return Localization.DatabaseAccess.Localization.AccountSettingsForSalePaymentSuccessNotFound;
                    }
                    SetEntries(FinancialYearID,
                        saleAccount.AccountHeadID.ToString(),
                        saleAccount.AccountControlID.ToString(),
                        saleAccount.AccountSubControlID.ToString(),
                        payInvoiceNo,
                        UserID.ToString(),
                        "0",
                        Amount.ToString(),
                        DateTime.Now,
                        CustomerName + $", Sale Payment Is Succeed");

                    _saleEntryFacade.SaleRepository.SetEntries(_dtEntries);

                    // Insert payment record
                    return await _saleEntryFacade.SaleRepository.ConfirmSale(CompanyID, BranchID, UserID, CustomerInvoiceID, Amount, CustomerID, payInvoiceNo, 0);
                }

                _saleEntryFacade.SaleRepository.SetEntries(_dtEntries);

                return await _saleEntryFacade.SaleRepository.InsertTransaction(CompanyID, BranchID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                return Localization.DatabaseAccess.Localization.UnexpectedErrorOccurred;
            }
        }

        public async Task<string> SalePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, float TotalAmount, float Amount, string CustomerID, string CustomerName, float RemainingBalance)
        {
            try
            {
                _dtEntries = new DataTable();

                string payInvoiceNo = "INP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                string saleTitle = Localization.DatabaseAccess.Localization.SaleTo + CustomerName.Trim();

                // Retrieve the active financial year
                var financialYearCheck = await _saleEntryFacade.FinancialYearRepository.GetSingleActiveAsync();
                string FinancialYearID = financialYearCheck != null ? Convert.ToString(financialYearCheck) : string.Empty;
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return Localization.DatabaseAccess.Localization.CompanyFinancialYearNotSet;
                }

                // Credit Entry Sale Payment Paid
                var saleAccount = await _saleEntryFacade.AccountSettingRepository.GetByActivityAsync(SALE_PAYMENT_PAID_ACTIVITY_ID, CompanyID, BranchID);
                if (saleAccount == null)
                {
                    return Localization.DatabaseAccess.Localization.AccountSettingsForSalePaymentPaidNotFound;
                }
                SetEntries(FinancialYearID,
                    saleAccount.AccountHeadID.ToString(),
                    saleAccount.AccountControlID.ToString(),
                    saleAccount.AccountSubControlID.ToString(),
                    InvoiceNo,
                    UserID.ToString(),
                    Amount.ToString(),
                    "0",
                    DateTime.Now,
                    $"Sale Payment Paid By " + CustomerName);

                // Debit Entry Sale Payment Success
                saleAccount = await _saleEntryFacade.AccountSettingRepository.GetByActivityAsync(SALE_PAYMENT_SUCCESS_ACTIVITY_ID, CompanyID, BranchID);
                if (saleAccount == null)
                {
                    return Localization.DatabaseAccess.Localization.AccountSettingsForSalePaymentSuccessNotFound;
                }
                SetEntries(FinancialYearID,
                    saleAccount.AccountHeadID.ToString(),
                    saleAccount.AccountControlID.ToString(),
                    saleAccount.AccountSubControlID.ToString(),
                    InvoiceNo,
                    UserID.ToString(),
                    "0",
                    Amount.ToString(),
                    DateTime.Now,
                    CustomerName + $", Sale Payment Is Succeed");

                _saleEntryFacade.SaleRepository.SetEntries(_dtEntries);
                // Insert payment record
                await _saleEntryFacade.SaleRepository.ConfirmSale(CompanyID, BranchID, UserID, CustomerInvoiceID, Amount, CustomerID, payInvoiceNo, RemainingBalance);
                // Insert transaction
                await _saleEntryFacade.SaleRepository.InsertTransaction(CompanyID, BranchID);

                return Localization.DatabaseAccess.Localization.PaidSuccessfully;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                return Localization.DatabaseAccess.Localization.UnexpectedErrorOccurred;
            }
        }

        // Sale Return
        public async Task<string> ReturnSale(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, int CustomerReturnInvoiceID, float Amount, string CustomerID, string Customername, bool isPayment)
        {
            try
            {
                _dtEntries = new DataTable();

                string returnSaleTitle = Localization.DatabaseAccess.Localization.ReturnSaleFrom + Customername.Trim();

                // Retrieve the active financial year
                var financialYearCheck = await _saleEntryFacade.FinancialYearRepository.GetSingleActiveAsync();
                string FinancialYearID = financialYearCheck != null ? Convert.ToString(financialYearCheck) : string.Empty;
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return Localization.DatabaseAccess.Localization.CompanyFinancialYearNotSet;
                }

                // Debit Entry Return Sale
                var returnSaleAccount = await _saleEntryFacade.AccountSettingRepository.GetByActivityAsync(SALE_RETURN_ACTIVITY_ID, CompanyID, BranchID);
                if (returnSaleAccount == null)
                {
                    return Localization.DatabaseAccess.Localization.AccountSettingsForSaleReturnNotFound;
                }
                SetEntries(FinancialYearID,
                    returnSaleAccount.AccountHeadID.ToString(),
                    returnSaleAccount.AccountControlID.ToString(),
                    returnSaleAccount.AccountSubControlID.ToString(),
                    InvoiceNo,
                    UserID.ToString(),
                    "0",
                    Amount.ToString(),
                    DateTime.Now,
                    returnSaleTitle);

                // Credit Entry Return Sale
                returnSaleAccount = await _saleEntryFacade.AccountSettingRepository.GetByActivityAsync(SALE_RETURN_PAYMENT_PENDING_ACTIVITY_ID, CompanyID, BranchID);
                if (returnSaleAccount == null)
                {
                    return Localization.DatabaseAccess.Localization.AccountSettingsForSaleReturnPaymentPendingNotFound;
                }
                SetEntries(FinancialYearID,
                    returnSaleAccount.AccountHeadID.ToString(),
                    returnSaleAccount.AccountControlID.ToString(),
                    returnSaleAccount.AccountSubControlID.ToString(),
                    InvoiceNo,
                    UserID.ToString(),
                    Amount.ToString(),
                    "0",
                    DateTime.Now,
                    Customername + $", .Return Sale Payment Is Pending");

                if (isPayment)
                {
                    string payInvoiceNo = "RIP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                    // Credit Entry Return Sale Payment Paid
                    returnSaleAccount = await _saleEntryFacade.AccountSettingRepository.GetByActivityAsync(SALE_RETURN_PAYMENT_PAID_ACTIVITY_ID, CompanyID, BranchID);
                    if (returnSaleAccount == null)
                    {
                        return Localization.DatabaseAccess.Localization.AccountSettingsForSaleReturnPaymentPaidNotFound;
                    }
                    SetEntries(FinancialYearID,
                        returnSaleAccount.AccountHeadID.ToString(),
                        returnSaleAccount.AccountControlID.ToString(),
                        returnSaleAccount.AccountSubControlID.ToString(),
                        payInvoiceNo,
                        UserID.ToString(),
                        "0",
                        Amount.ToString(),
                        DateTime.Now,
                        $"{Localization.DatabaseAccess.Localization.ReturnSalePaymentPaidTo} " + Customername);

                    // Debit Entry Return Sale Payment Success
                    returnSaleAccount = await _saleEntryFacade.AccountSettingRepository.GetByActivityAsync(SALE_RETURN_PAYMENT_SUCCESS_ACTIVITY_ID, CompanyID, BranchID);
                    if (returnSaleAccount == null)
                    {
                        return Localization.DatabaseAccess.Localization.AccountSettingsForSalePaymentSuccessNotFound;
                    }
                    SetEntries(FinancialYearID,
                        returnSaleAccount.AccountHeadID.ToString(),
                        returnSaleAccount.AccountControlID.ToString(),
                        returnSaleAccount.AccountSubControlID.ToString(),
                        payInvoiceNo,
                        UserID.ToString(),
                        "0",
                        Amount.ToString(),
                        DateTime.Now,
                        Customername + $", Return Sale Payment Is Succeed");

                    _saleEntryFacade.SaleRepository.SetEntries(_dtEntries);
                    return await _saleEntryFacade.SaleRepository.ReturnSale(CompanyID, BranchID, UserID, CustomerInvoiceID, CustomerReturnInvoiceID, Amount, CustomerID, payInvoiceNo, 0);
                }

                _saleEntryFacade.SaleRepository.SetEntries(_dtEntries);
                return await _saleEntryFacade.SaleRepository.InsertTransaction(CompanyID, BranchID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                return Localization.DatabaseAccess.Localization.UnexpectedErrorOccurred;
            }
        }

        public async Task<string> ReturnSalePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, int CustomerReturnInvoiceID, float TotalAmount, float Amount, string CustomerID, string Customername, float RemainingBalance)
        {
            try
            {
                _dtEntries = new DataTable();

                string saleTitle = Localization.DatabaseAccess.Localization.ReturnSaleFrom + Customername.Trim();
                string payInvoiceNo = "RIP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                // Retrieve the active financial year
                var financialYearCheck = await _saleEntryFacade.FinancialYearRepository.GetSingleActiveAsync();
                string FinancialYearID = financialYearCheck != null ? Convert.ToString(financialYearCheck) : string.Empty;
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return Localization.DatabaseAccess.Localization.CompanyFinancialYearNotSet;
                }

                string transactionTitle;

                // Credit Entry Return Sale Payment Paid
                var returnSaleAccount = await _saleEntryFacade.AccountSettingRepository.GetByActivityAsync(SALE_RETURN_PAYMENT_PAID_ACTIVITY_ID, CompanyID, BranchID);
                if (returnSaleAccount == null)
                {
                    return Localization.DatabaseAccess.Localization.AccountSettingsForSaleReturnPaymentPaidNotFound;
                }

                transactionTitle = Localization.DatabaseAccess.Localization.ReturnSalePaymentPaidTo + Customername;
                SetEntries(FinancialYearID,
                    returnSaleAccount.AccountHeadID.ToString(),
                    returnSaleAccount.AccountControlID.ToString(),
                    returnSaleAccount.AccountSubControlID.ToString(),
                    InvoiceNo,
                    UserID.ToString(),
                    "0",
                    Amount.ToString(),
                    DateTime.Now,
                    transactionTitle);

                // Debit Entry Return Sale Payment Success
                returnSaleAccount = await _saleEntryFacade.AccountSettingRepository.GetByActivityAsync(SALE_RETURN_PAYMENT_SUCCESS_ACTIVITY_ID, CompanyID, BranchID);
                if (returnSaleAccount == null)
                {
                    return Localization.DatabaseAccess.Localization.AccountSettingsForSaleReturnPaymentSuccessNotFound;
                }
                transactionTitle = Customername + Localization.DatabaseAccess.Localization.ReturnSalePaymentSsSucceed;
                SetEntries(FinancialYearID,
                    returnSaleAccount.AccountHeadID.ToString(),
                    returnSaleAccount.AccountControlID.ToString(),
                    returnSaleAccount.AccountSubControlID.ToString(),
                    InvoiceNo,
                    UserID.ToString(),
                    Amount.ToString(),
                    "0",
                    DateTime.Now,
                    transactionTitle);

                _saleEntryFacade.SaleRepository.SetEntries(_dtEntries);
                // Insert return payment record
                await _saleEntryFacade.SaleRepository.ReturnSalePayment(CompanyID, BranchID, UserID, InvoiceNo, CustomerInvoiceID, CustomerReturnInvoiceID, TotalAmount, Amount, CustomerID, RemainingBalance);
                // Insert transaction records
                await _saleEntryFacade.SaleRepository.InsertTransaction(CompanyID, BranchID);

                return Localization.DatabaseAccess.Localization.PaidSuccessfully;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                return Localization.DatabaseAccess.Localization.UnexpectedErrorOccurred;
            }
        }

        public async Task CompleteSale(IEnumerable<SaleCartDetail> saleDetails)
        {
            foreach (var item in saleDetails)
            {
                var stockItem = await _saleEntryFacade.StockRepository.GetByIdAsync(item.ProductID);
                if (stockItem != null)
                {
                    stockItem.Quantity += item.SaleQuantity;
                    await _saleEntryFacade.StockRepository.UpdateAsync(stockItem);
                }
                await _saleEntryFacade.SaleCartDetailRepository.UpdateAsync(item);
            }
        }

        private void SetEntries(string FinancialYearID, string AccountHeadID, string AccountControlID, string AccountSubControlID, string InvoiceNo, string UserID, string Credit, string Debit, DateTime TransactionDate, string TransectionTitle)
        {
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
