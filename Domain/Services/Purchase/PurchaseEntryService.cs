using Domain.RepositoryAccess;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Domain.Services.Purchase
{
    public interface IPurchaseEntryService
    {
        Task<string> ConfirmPurchase(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, float Amount, string SupplierID, string SupplierName, bool isPayment);
        Task<string> PurchasePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, float TotalAmount, float Amount, string SupplierID, string SupplierName, float RemainingBalance);
        Task<string> ReturnPurchase(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, int SupplierReturnInvoiceID, float Amount, string SupplierID, string SupplierName, bool isPayment);
        Task<string> ReturnPurchasePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, int SupplierReturnInvoiceID, float TotalAmount, float Amount, string SupplierID, string SupplierName, float RemainingBalance);
    }

    public class PurchaseEntryService : IPurchaseEntryService
    {
        private readonly IAccountSettingRepository _accountSettingRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IFinancialYearRepository _financialYearRepository;

        private readonly string selectsupplierid = string.Empty;
        private readonly DataTable _dtEntries = null;

        private const int PURCHASE_ACCOUNT_CONTROL_ID = 1;
        private const int PURCHASE_PAYMENT_PENDING_CONTROL_ID = 2;
        private const int PURCHASE_PAYMENT_PAID_CONTROL_ID = 3;
        private const int PURCHASE_PAYMENT_SUCCESS_CONTROL_ID = 4;
        private const int PURCHASE_RETURN_CONTROL_ID = 5;
        private const int PURCHASE_RETURN_PAYMENT_PENDING_CONTROL_ID = 6;
        private const int PURCHASE_RETURN_PAYMENT_SUCCESS_CONTROL_ID = 7;

        public PurchaseEntryService(
            IAccountSettingRepository accountSettingRepository, 
            IPurchaseRepository purchaseRepository, 
            IFinancialYearRepository financialYearRepository)
        {
            _accountSettingRepository = accountSettingRepository ?? throw new ArgumentNullException(nameof(IAccountSettingRepository));
            _purchaseRepository = purchaseRepository ?? throw new ArgumentNullException(nameof(IPurchaseRepository));
            _financialYearRepository = financialYearRepository ?? throw new ArgumentNullException(nameof(IFinancialYearRepository));
        }

        public async Task<string> ConfirmPurchase(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, float Amount, string SupplierID, string SupplierName, bool isPayment)
        {
            string purchaseTitle = Localization.Localization.PurchaseFrom + SupplierName.Trim();

            // Retrieve the active financial year
            var financialYearCheck = await _financialYearRepository.GetSingleActiveAsync();
            string FinancialYearID = financialYearCheck != null ? Convert.ToString(financialYearCheck) : string.Empty;
            if (string.IsNullOrEmpty(FinancialYearID))
            {
                return Localization.Localization.CompanyFinancialYearNotSet;
            }

            // Debit Entry Purchase
            var purchaseAccount = await _accountSettingRepository.GetByActivityAsync(PURCHASE_ACCOUNT_CONTROL_ID, CompanyID, BranchID);
            if (purchaseAccount == null)
            {
                return Localization.Localization.AccountSettingsForPurchaseNotFound;
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
            purchaseAccount = await _accountSettingRepository.GetByActivityAsync(PURCHASE_PAYMENT_PENDING_CONTROL_ID, CompanyID, BranchID);
            if (purchaseAccount == null)
            {
                return Localization.Localization.AccountSettingsForPurchasePaymentPendingNotFound;
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
                SupplierName + $", {Localization.Localization.ReturnPurchasePaymentIsPending}");

            if (isPayment)
            {
                string payInvoiceNo = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                // Debit Entry Purchase Payment Paid
                purchaseAccount = await _accountSettingRepository.GetByActivityAsync(PURCHASE_PAYMENT_PAID_CONTROL_ID, CompanyID, BranchID);
                if (purchaseAccount == null)
                {
                    return Localization.Localization.AccountSettingsForPurchasePaymentPaidNotFound;
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
                    $"{Localization.Localization.PaymentPaidTo} " + SupplierName);

                // Credit Entry Purchase Payment Success
                purchaseAccount = await _accountSettingRepository.GetByActivityAsync(PURCHASE_PAYMENT_SUCCESS_CONTROL_ID, CompanyID, BranchID);
                if (purchaseAccount == null)
                {
                    return Localization.Localization.AccountSettingsForPurchasePaymentSuccessNotFound;
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
                    SupplierName + $", {Localization.Localization.PurchasePaymentIsSucceed}");

                return await _purchaseRepository.ConfirmPurchase(CompanyID, BranchID, UserID, SupplierInvoiceID, Amount, SupplierID, payInvoiceNo, 0);
            }

            return await _purchaseRepository.InsertTransaction(CompanyID, BranchID);
        }

        public async Task<string> PurchasePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, float TotalAmount, float Amount, string SupplierID, string SupplierName, float RemainingBalance)
        {
            try
            {
                string pruchaseTitle = Localization.Localization.PurchaseFrom + SupplierName.Trim();

                // Retrieve the active financial year
                var financialYearCheck = await _financialYearRepository.GetSingleActiveAsync();
                string FinancialYearID = financialYearCheck != null ? Convert.ToString(financialYearCheck) : string.Empty;
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return Localization.Localization.CompanyFinancialYearNotSet;
                }

                // Debit Entry Purchase
                var purchaseAccount = await _accountSettingRepository.GetByActivityAsync(PURCHASE_ACCOUNT_CONTROL_ID, CompanyID, BranchID);
                if (purchaseAccount == null)
                {
                    return Localization.Localization.AccountSettingsForPurchaseNotFound;
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
                purchaseAccount = await _accountSettingRepository.GetByActivityAsync(PURCHASE_PAYMENT_PENDING_CONTROL_ID, CompanyID, BranchID);
                if (purchaseAccount == null)
                {
                    return Localization.Localization.AccountSettingsForPurchasePaymentPendingNotFound;
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
                    SupplierName + $", {Localization.Localization.ReturnPurchasePaymentIsPending}");

                // Payment Process
                if (Amount > 0)
                {
                    string payInvoiceNo = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                    // Credit Entry Purchase Payment Paid
                    purchaseAccount = await _accountSettingRepository.GetByActivityAsync(PURCHASE_PAYMENT_PAID_CONTROL_ID, CompanyID, BranchID);
                    if (purchaseAccount == null)
                    {
                        return Localization.Localization.AccountSettingsForPurchasePaymentPaidNotFound;
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
                        $"{Localization.Localization.PaymentPaidTo} " + SupplierName);

                    // Debit Entry Purchase Payment Success
                    purchaseAccount = await _accountSettingRepository.GetByActivityAsync(PURCHASE_PAYMENT_SUCCESS_CONTROL_ID, CompanyID, BranchID);
                    if (purchaseAccount == null)
                    {
                        return Localization.Localization.AccountSettingsForPurchasePaymentSuccessNotFound;
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
                        SupplierName + $", {Localization.Localization.PurchasePaymentIsSucceed}");

                    // Insert payment record
                    return await _purchaseRepository.ConfirmPurchase(CompanyID, BranchID, UserID, SupplierInvoiceID, Amount, SupplierID, payInvoiceNo, RemainingBalance);
                }

                // Insert transaction records
                return await _purchaseRepository.InsertTransaction(CompanyID, BranchID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                return Localization.Localization.UnexpectedErrorOccurred;
            }
        }

        // Purchase Return
        public async Task<string> ReturnPurchase(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, int SupplierReturnInvoiceID, float Amount, string SupplierID, string SupplierName, bool isPayment)
        {
            string returnPurchaseTitle = Localization.Localization.ReturnPurchaseTo + SupplierName.Trim();

            // Retrieve the active financial year
            var financialYearCheck = await _financialYearRepository.GetSingleActiveAsync();
            string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck) : string.Empty);
            if (string.IsNullOrEmpty(FinancialYearID))
            {
                return Localization.Localization.CompanyFinancialYearNotSet;
            }

            string successMessage = Localization.Localization.ReturnPurchaseSuccess;

            // Credit Entry Return Purchase
            var returnPurchaseAccount = await _accountSettingRepository.GetByActivityAsync(PURCHASE_RETURN_CONTROL_ID, CompanyID, BranchID);
            if (returnPurchaseAccount == null)
            {
                return Localization.Localization.AccountSettingsАForReturnPurchaseNotFound;
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
            returnPurchaseAccount = await _accountSettingRepository.GetByActivityAsync(PURCHASE_RETURN_PAYMENT_PENDING_CONTROL_ID, CompanyID, BranchID);
            if (returnPurchaseAccount == null)
            {
                return Localization.Localization.AccountSettingsForPurchaseReturnPaymentPendingNotFound;
            }
            string pendingPaymentTitle = SupplierName + $", {Localization.Localization.ReturnPurchasePaymentIsSucceed}";
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
                returnPurchaseAccount = await _accountSettingRepository.GetByActivityAsync(PURCHASE_RETURN_PAYMENT_PENDING_CONTROL_ID, CompanyID, BranchID);
                if (returnPurchaseAccount == null)
                {
                    return Localization.Localization.AccountSettingsForPurchaseReturnPaymentPendingNotFound;
                }
                string paymentFromTitle = $"{Localization.Localization.ReturnPaymentFrom} " + SupplierName;
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
                returnPurchaseAccount = await _accountSettingRepository.GetByActivityAsync(PURCHASE_RETURN_PAYMENT_SUCCESS_CONTROL_ID, CompanyID, BranchID);
                if (returnPurchaseAccount == null)
                {
                    return Localization.Localization.AccountSettingsForPurchaseReturnPaymentSucceedNotFound;
                }
                string paymentSuccessTitle = SupplierName + Localization.Localization.ReturnPurchasePaymentIsSucceed;
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

                successMessage += await _purchaseRepository.ConfirmPurchaseReturn(CompanyID, BranchID, UserID, SupplierInvoiceID, SupplierReturnInvoiceID, Amount, SupplierID, payInvoiceNo);
            }

            // Insert transaction records
            await _purchaseRepository.InsertTransaction(CompanyID, BranchID);

            return successMessage;
        }

        public async Task<string> ReturnPurchasePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, int SupplierReturnInvoiceID, float TotalAmount, float Amount, string SupplierID, string SupplierName, float RemainingBalance)
        {
            try
            {
                string returnPurchaseTitle = Localization.Localization.ReturnPurchaseTo + SupplierName.Trim();
                string payInvoiceNo = "RPP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                // Retrieve the active financial year
                var financialYearCheck = await _financialYearRepository.GetSingleActiveAsync();
                string FinancialYearID = financialYearCheck != null ? Convert.ToString(financialYearCheck) : string.Empty;

                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return Localization.Localization.CompanyFinancialYearNotSet;
                }

                string AccountHeadID = string.Empty;
                string AccountControlID = string.Empty;
                string AccountSubControlID = string.Empty;
                string transactionTitle = string.Empty;

                // Retrieve account settings for Purchase Return Payment Pending
                var returnPurchaseAccount = await _accountSettingRepository.GetByActivityAsync(PURCHASE_RETURN_PAYMENT_PENDING_CONTROL_ID, CompanyID, BranchID);
                if (returnPurchaseAccount == null)
                {
                    return Localization.Localization.AccountSettingsForPurchaseReturnPaymentPendingNotFound;
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
                    $"{Localization.Localization.ReturnPaymentFrom} " + SupplierName);

                // Retrieve account settings for Purchase Return Payment Succeed
                returnPurchaseAccount = await _accountSettingRepository.GetByActivityAsync(PURCHASE_RETURN_PAYMENT_SUCCESS_CONTROL_ID, CompanyID, BranchID);
                if (returnPurchaseAccount == null)
                {
                    return Localization.Localization.AccountSettingsForPurchaseReturnPaymentSucceedNotFound;
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
                    SupplierName + $", {Localization.Localization.ReturnPurchasePaymentIsSucceed}");

                // Insert supplier return payment record
                await _purchaseRepository.ConfirmPurchaseReturn(CompanyID, BranchID, UserID, SupplierInvoiceID, SupplierReturnInvoiceID, Amount, SupplierID, payInvoiceNo);

                // Insert transaction entries
                await _purchaseRepository.InsertTransaction(CompanyID, BranchID);

                return Localization.Localization.ReturnPurchasePaymentIsSucceed;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                return Localization.Localization.UnexpectedErrorOccurred;
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
