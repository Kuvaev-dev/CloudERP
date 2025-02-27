using Domain.Models;
using Domain.ServiceAccess;
using Services.Facades;
using System.Data;

namespace Services.Implementations
{
    public class PurchaseEntryService : IPurchaseEntryService
    {
        private readonly PurchaseEntryFacade _purchaseEntryFacade;

        private DataTable _dtEntries;

        private const int PURCHASE_ACCOUNT_ACTIVITY_ID = 1;
        private const int PURCHASE_PAYMENT_PENDING_ACTIVITY_ID = 2;
        private const int PURCHASE_PAYMENT_PAID_ACTIVITY_ID = 3;
        private const int PURCHASE_PAYMENT_SUCCESS_ACTIVITY_ID = 4;
        private const int PURCHASE_RETURN_ACTIVITY_ID = 5;
        private const int PURCHASE_RETURN_PAYMENT_PENDING_ACTIVITY_ID = 6;
        private const int PURCHASE_RETURN_PAYMENT_SUCCESS_ACTIVITY_ID = 7;

        public PurchaseEntryService(PurchaseEntryFacade purchaseEntryFacade)
        {
            _purchaseEntryFacade = purchaseEntryFacade ?? throw new ArgumentNullException(nameof(purchaseEntryFacade));
        }

        public async Task<string> ConfirmPurchase(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, float Amount, string SupplierID, string SupplierName, bool isPayment)
        {
            _dtEntries = new DataTable();
            string purchaseTitle = Localization.DatabaseAccess.Localization.PurchaseFrom + SupplierName.Trim();

            // Retrieve the active financial year
            var financialYearCheck = await _purchaseEntryFacade.FinancialYearRepository.GetSingleActiveAsync();
            string FinancialYearID = financialYearCheck != null ? Convert.ToString(financialYearCheck) : string.Empty;
            if (string.IsNullOrEmpty(FinancialYearID))
            {
                return Localization.DatabaseAccess.Localization.CompanyFinancialYearNotSet;
            }

            // Debit Entry Purchase
            var purchaseAccount = await _purchaseEntryFacade.AccountSettingRepository.GetByActivityAsync(PURCHASE_ACCOUNT_ACTIVITY_ID, CompanyID, BranchID);
            if (purchaseAccount == null)
            {
                return Localization.DatabaseAccess.Localization.AccountSettingsForPurchaseNotFound;
            }
            SetEntries(FinancialYearID,
                purchaseAccount.AccountHeadID.ToString(),
                purchaseAccount.AccountControlID.ToString(),
                purchaseAccount.AccountSubControlID.ToString(),
                InvoiceNo, UserID.ToString(),
                "0",
                Amount.ToString(),
                DateTime.Now,
                purchaseTitle);

            // Credit Entry Purchase Payment Pending
            purchaseAccount = await _purchaseEntryFacade.AccountSettingRepository.GetByActivityAsync(PURCHASE_PAYMENT_PENDING_ACTIVITY_ID, CompanyID, BranchID);
            if (purchaseAccount == null)
            {
                return Localization.DatabaseAccess.Localization.AccountSettingsForPurchasePaymentPendingNotFound;
            }
            SetEntries(FinancialYearID,
                purchaseAccount.AccountHeadID.ToString(),
                purchaseAccount.AccountControlID.ToString(),
                purchaseAccount.AccountSubControlID.ToString(),
                InvoiceNo,
                UserID.ToString(),
                Amount.ToString(),
                "0",
                DateTime.Now,
                SupplierName + $", {Localization.DatabaseAccess.Localization.ReturnPurchasePaymentIsPending}");

            if (isPayment)
            {
                string payInvoiceNo = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                // Debit Entry Purchase Payment Paid
                purchaseAccount = await _purchaseEntryFacade.AccountSettingRepository.GetByActivityAsync(PURCHASE_PAYMENT_PAID_ACTIVITY_ID, CompanyID, BranchID);
                if (purchaseAccount == null)
                {
                    return Localization.DatabaseAccess.Localization.AccountSettingsForPurchasePaymentPaidNotFound;
                }
                SetEntries(FinancialYearID,
                    purchaseAccount.AccountHeadID.ToString(),
                    purchaseAccount.AccountControlID.ToString(),
                    purchaseAccount.AccountSubControlID.ToString(),
                    payInvoiceNo,
                    UserID.ToString(),
                    "0",
                    Amount.ToString(),
                    DateTime.Now,
                    $"Oayment Paid To: " + SupplierName);

                // Credit Entry Purchase Payment Success
                purchaseAccount = await _purchaseEntryFacade.AccountSettingRepository.GetByActivityAsync(PURCHASE_PAYMENT_SUCCESS_ACTIVITY_ID, CompanyID, BranchID);
                if (purchaseAccount == null)
                {
                    return Localization.DatabaseAccess.Localization.AccountSettingsForPurchasePaymentSuccessNotFound;
                }
                SetEntries(FinancialYearID,
                    purchaseAccount.AccountHeadID.ToString(),
                    purchaseAccount.AccountControlID.ToString(),
                    purchaseAccount.AccountSubControlID.ToString(),
                    payInvoiceNo,
                    UserID.ToString(),
                    Amount.ToString(),
                    "0",
                    DateTime.Now,
                    SupplierName + $", Purchase Payment Is Succeed");

                _purchaseEntryFacade.PurchaseRepository.SetEntries(_dtEntries);

                return await _purchaseEntryFacade.PurchaseRepository.ConfirmPurchase(CompanyID, BranchID, UserID, SupplierInvoiceID, Amount, SupplierID, payInvoiceNo, 0);
            }

            _purchaseEntryFacade.PurchaseRepository.SetEntries(_dtEntries);

            return await _purchaseEntryFacade.PurchaseRepository.InsertTransaction(CompanyID, BranchID);
        }

        public async Task<string> PurchasePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, float TotalAmount, float Amount, string SupplierID, string SupplierName, float RemainingBalance)
        {
            try
            {
                _dtEntries = new DataTable();

                string pruchaseTitle = Localization.DatabaseAccess.Localization.PurchaseFrom + SupplierName.Trim();

                // Retrieve the active financial year
                var financialYearCheck = await _purchaseEntryFacade.FinancialYearRepository.GetSingleActiveAsync();
                string FinancialYearID = financialYearCheck != null ? Convert.ToString(financialYearCheck) : string.Empty;
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return Localization.DatabaseAccess.Localization.CompanyFinancialYearNotSet;
                }

                // Debit Entry Purchase
                var purchaseAccount = await _purchaseEntryFacade.AccountSettingRepository.GetByActivityAsync(PURCHASE_ACCOUNT_ACTIVITY_ID, CompanyID, BranchID);
                if (purchaseAccount == null)
                {
                    return Localization.DatabaseAccess.Localization.AccountSettingsForPurchaseNotFound;
                }
                SetEntries(FinancialYearID,
                    purchaseAccount.AccountHeadID.ToString(),
                    purchaseAccount.AccountControlID.ToString(),
                    purchaseAccount.AccountSubControlID.ToString(),
                    InvoiceNo,
                    UserID.ToString(),
                    "0",
                    Amount.ToString(),
                    DateTime.Now,
                    pruchaseTitle);

                // Credit Entry Purchase Payment Pending
                purchaseAccount = await _purchaseEntryFacade.AccountSettingRepository.GetByActivityAsync(PURCHASE_PAYMENT_PENDING_ACTIVITY_ID, CompanyID, BranchID);
                if (purchaseAccount == null)
                {
                    return Localization.DatabaseAccess.Localization.AccountSettingsForPurchasePaymentPendingNotFound;
                }
                SetEntries(FinancialYearID,
                    purchaseAccount.AccountHeadID.ToString(),
                    purchaseAccount.AccountControlID.ToString(),
                    purchaseAccount.AccountSubControlID.ToString(),
                    InvoiceNo,
                    UserID.ToString(),
                    Amount.ToString(),
                    "0",
                    DateTime.Now,
                    SupplierName + $", {Localization.DatabaseAccess.Localization.ReturnPurchasePaymentIsPending}");

                // Payment Process
                if (Amount > 0)
                {
                    string payInvoiceNo = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                    // Credit Entry Purchase Payment Paid
                    purchaseAccount = await _purchaseEntryFacade.AccountSettingRepository.GetByActivityAsync(PURCHASE_PAYMENT_PAID_ACTIVITY_ID, CompanyID, BranchID);
                    if (purchaseAccount == null)
                    {
                        return Localization.DatabaseAccess.Localization.AccountSettingsForPurchasePaymentPaidNotFound;
                    }
                    SetEntries(FinancialYearID,
                        purchaseAccount.AccountHeadID.ToString(),
                        purchaseAccount.AccountControlID.ToString(),
                        purchaseAccount.AccountSubControlID.ToString(),
                        payInvoiceNo,
                        UserID.ToString(),
                        "0",
                        Amount.ToString(),
                        DateTime.Now,
                        $"Payment Paid To " + SupplierName);

                    // Debit Entry Purchase Payment Success
                    purchaseAccount = await _purchaseEntryFacade.AccountSettingRepository.GetByActivityAsync(PURCHASE_PAYMENT_SUCCESS_ACTIVITY_ID, CompanyID, BranchID);
                    if (purchaseAccount == null)
                    {
                        return Localization.DatabaseAccess.Localization.AccountSettingsForPurchasePaymentSuccessNotFound;
                    }
                    SetEntries(FinancialYearID,
                        purchaseAccount.AccountHeadID.ToString(),
                        purchaseAccount.AccountControlID.ToString(),
                        purchaseAccount.AccountSubControlID.ToString(),
                        payInvoiceNo,
                        UserID.ToString(),
                        Amount.ToString(),
                        "0",
                        DateTime.Now,
                        SupplierName + $", Purchase Payment Is Succeed");

                    _purchaseEntryFacade.PurchaseRepository.SetEntries(_dtEntries);

                    // Insert payment record
                    return await _purchaseEntryFacade.PurchaseRepository.ConfirmPurchase(CompanyID, BranchID, UserID, SupplierInvoiceID, Amount, SupplierID, payInvoiceNo, RemainingBalance);
                }

                _purchaseEntryFacade.PurchaseRepository.SetEntries(_dtEntries);

                // Insert transaction records
                return await _purchaseEntryFacade.PurchaseRepository.InsertTransaction(CompanyID, BranchID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                return Localization.DatabaseAccess.Localization.UnexpectedErrorOccurred;
            }
        }

        // Purchase Return
        public async Task<string> ReturnPurchase(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, int SupplierReturnInvoiceID, float Amount, string SupplierID, string SupplierName, bool isPayment)
        {
            _dtEntries = new DataTable();

            string returnPurchaseTitle = Localization.DatabaseAccess.Localization.ReturnPurchaseTo + SupplierName.Trim();

            // Retrieve the active financial year
            var financialYearCheck = await _purchaseEntryFacade.FinancialYearRepository.GetSingleActiveAsync();
            string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck) : string.Empty);
            if (string.IsNullOrEmpty(FinancialYearID))
            {
                return Localization.DatabaseAccess.Localization.CompanyFinancialYearNotSet;
            }

            string successMessage = Localization.DatabaseAccess.Localization.ReturnPurchaseSuccess;

            // Credit Entry Return Purchase
            var returnPurchaseAccount = await _purchaseEntryFacade.AccountSettingRepository.GetByActivityAsync(PURCHASE_RETURN_ACTIVITY_ID, CompanyID, BranchID);
            if (returnPurchaseAccount == null)
            {
                return Localization.DatabaseAccess.Localization.AccountSettingsАForReturnPurchaseNotFound;
            }
            SetEntries(FinancialYearID,
                returnPurchaseAccount.AccountHeadID.ToString(),
                returnPurchaseAccount.AccountControlID.ToString(),
                returnPurchaseAccount.AccountSubControlID.ToString(),
                InvoiceNo,
                UserID.ToString(),
                Amount.ToString(),
                "0",
                DateTime.Now,
                returnPurchaseTitle);

            // Debit Entry Return Purchase Payment Pending
            returnPurchaseAccount = await _purchaseEntryFacade.AccountSettingRepository.GetByActivityAsync(PURCHASE_RETURN_PAYMENT_PENDING_ACTIVITY_ID, CompanyID, BranchID);
            if (returnPurchaseAccount == null)
            {
                return Localization.DatabaseAccess.Localization.AccountSettingsForPurchaseReturnPaymentPendingNotFound;
            }
            string pendingPaymentTitle = SupplierName + $", {Localization.DatabaseAccess.Localization.ReturnPurchasePaymentIsSucceed}";
            SetEntries(FinancialYearID,
                returnPurchaseAccount.AccountHeadID.ToString(),
                returnPurchaseAccount.AccountControlID.ToString(),
                returnPurchaseAccount.AccountSubControlID.ToString(),
                InvoiceNo,
                UserID.ToString(),
                "0",
                Amount.ToString(),
                DateTime.Now,
                pendingPaymentTitle);

            if (isPayment)
            {
                string payInvoiceNo = "RPP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                // Debit Entry Return Payment from Supplier
                returnPurchaseAccount = await _purchaseEntryFacade.AccountSettingRepository.GetByActivityAsync(PURCHASE_RETURN_PAYMENT_PENDING_ACTIVITY_ID, CompanyID, BranchID);
                if (returnPurchaseAccount == null)
                {
                    return Localization.DatabaseAccess.Localization.AccountSettingsForPurchaseReturnPaymentPendingNotFound;
                }
                string paymentFromTitle = $"{Localization.DatabaseAccess.Localization.ReturnPaymentFrom} " + SupplierName;
                SetEntries(FinancialYearID,
                    returnPurchaseAccount.AccountHeadID.ToString(),
                    returnPurchaseAccount.AccountControlID.ToString(),
                    returnPurchaseAccount.AccountSubControlID.ToString(),
                    payInvoiceNo,
                    UserID.ToString(),
                    Amount.ToString(),
                    "0",
                    DateTime.Now,
                    paymentFromTitle);

                // Credit Entry Purchase Return Payment Succeed
                returnPurchaseAccount = await _purchaseEntryFacade.AccountSettingRepository.GetByActivityAsync(PURCHASE_RETURN_PAYMENT_SUCCESS_ACTIVITY_ID, CompanyID, BranchID);
                if (returnPurchaseAccount == null)
                {
                    return Localization.DatabaseAccess.Localization.AccountSettingsForPurchaseReturnPaymentSucceedNotFound;
                }
                string paymentSuccessTitle = SupplierName + Localization.DatabaseAccess.Localization.ReturnPurchasePaymentIsSucceed;
                SetEntries(FinancialYearID,
                    returnPurchaseAccount.AccountHeadID.ToString(),
                    returnPurchaseAccount.AccountControlID.ToString(),
                    returnPurchaseAccount.AccountSubControlID.ToString(),
                    payInvoiceNo,
                    UserID.ToString(),
                    "0",
                    Amount.ToString(),
                    DateTime.Now,
                    paymentSuccessTitle);

                successMessage += await _purchaseEntryFacade.PurchaseRepository.ConfirmPurchaseReturn(CompanyID, BranchID, UserID, SupplierInvoiceID, SupplierReturnInvoiceID, Amount, SupplierID, payInvoiceNo);

                _purchaseEntryFacade.PurchaseRepository.SetEntries(_dtEntries);
            }

            _purchaseEntryFacade.PurchaseRepository.SetEntries(_dtEntries);

            // Insert transaction records
            await _purchaseEntryFacade.PurchaseRepository.InsertTransaction(CompanyID, BranchID);

            return successMessage;
        }

        public async Task<string> ReturnPurchasePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, int SupplierReturnInvoiceID, float TotalAmount, float Amount, string SupplierID, string SupplierName, float RemainingBalance)
        {
            try
            {
                string returnPurchaseTitle = Localization.DatabaseAccess.Localization.ReturnPurchaseTo + SupplierName.Trim();
                string payInvoiceNo = "RPP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                // Retrieve the active financial year
                var financialYearCheck = await _purchaseEntryFacade.FinancialYearRepository.GetSingleActiveAsync();
                string FinancialYearID = financialYearCheck != null ? Convert.ToString(financialYearCheck) : string.Empty;

                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return Localization.DatabaseAccess.Localization.CompanyFinancialYearNotSet;
                }

                string AccountHeadID = string.Empty;
                string AccountControlID = string.Empty;
                string AccountSubControlID = string.Empty;
                string transactionTitle = string.Empty;

                // Retrieve account settings for Purchase Return Payment Pending
                var returnPurchaseAccount = await _purchaseEntryFacade.AccountSettingRepository.GetByActivityAsync(PURCHASE_RETURN_PAYMENT_PENDING_ACTIVITY_ID, CompanyID, BranchID);
                if (returnPurchaseAccount == null)
                {
                    return Localization.DatabaseAccess.Localization.AccountSettingsForPurchaseReturnPaymentPendingNotFound;
                }
                SetEntries(FinancialYearID,
                    returnPurchaseAccount.AccountHeadID.ToString(),
                    returnPurchaseAccount.AccountControlID.ToString(),
                    returnPurchaseAccount.AccountSubControlID.ToString(),
                    InvoiceNo,
                    UserID.ToString(),
                    Amount.ToString(),
                    "0",
                    DateTime.Now,
                    $"{Localization.DatabaseAccess.Localization.ReturnPaymentFrom} " + SupplierName);

                // Retrieve account settings for Purchase Return Payment Succeed
                returnPurchaseAccount = await _purchaseEntryFacade.AccountSettingRepository.GetByActivityAsync(PURCHASE_RETURN_PAYMENT_SUCCESS_ACTIVITY_ID, CompanyID, BranchID);
                if (returnPurchaseAccount == null)
                {
                    return Localization.DatabaseAccess.Localization.AccountSettingsForPurchaseReturnPaymentSucceedNotFound;
                }
                SetEntries(FinancialYearID,
                    returnPurchaseAccount.AccountHeadID.ToString(),
                    returnPurchaseAccount.AccountControlID.ToString(),
                    returnPurchaseAccount.AccountSubControlID.ToString(),
                    InvoiceNo,
                    UserID.ToString(),
                    "0",
                    Amount.ToString(),
                    DateTime.Now,
                    SupplierName + $", {Localization.DatabaseAccess.Localization.ReturnPurchasePaymentIsSucceed}");

                _purchaseEntryFacade.PurchaseRepository.SetEntries(_dtEntries);

                // Insert supplier return payment record
                await _purchaseEntryFacade.PurchaseRepository.ConfirmPurchaseReturn(CompanyID, BranchID, UserID, SupplierInvoiceID, SupplierReturnInvoiceID, Amount, SupplierID, payInvoiceNo);

                // Insert transaction entries
                await _purchaseEntryFacade.PurchaseRepository.InsertTransaction(CompanyID, BranchID);

                return Localization.DatabaseAccess.Localization.ReturnPurchasePaymentIsSucceed;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                return Localization.DatabaseAccess.Localization.UnexpectedErrorOccurred;
            }

        }

        public async Task CompletePurchase(IEnumerable<PurchaseCartDetail> purchaseDetails)
        {
            foreach (var item in purchaseDetails)
            {
                var stockItem = await _purchaseEntryFacade.StockRepository.GetByIdAsync(item.ProductID);
                if (stockItem != null)
                {
                    stockItem.Quantity += item.PurchaseQuantity;
                    await _purchaseEntryFacade.StockRepository.UpdateAsync(stockItem);
                }
                await _purchaseEntryFacade.PurchaseCartDetailRepository.UpdateAsync(item);
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
