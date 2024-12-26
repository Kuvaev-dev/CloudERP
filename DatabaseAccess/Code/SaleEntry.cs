using Domain.EntryAccess;
using Domain.RepositoryAccess;
using System;
using System.Data;
using System.Threading.Tasks;

namespace DatabaseAccess.Code
{
    public class SaleEntry : ISaleEntry
    {
        private readonly IFinancialYearRepository _financialYearRepository;
        private readonly IAccountSettingRepository _accountSettingRepository;
        private readonly ISaleRepository _saleRepository;

        public string selectcustomerid = string.Empty;
        private readonly DataTable _dtEntries = null;

        public SaleEntry(IFinancialYearRepository financialYearRepository, IAccountSettingRepository accountSettingRepository, ISaleRepository saleRepository)
        {
            _financialYearRepository = financialYearRepository;
            _accountSettingRepository = accountSettingRepository;
            _saleRepository = saleRepository;
        }

        public async Task<string> ConfirmSale(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, float Amount, string CustomerID, string CustomerName, bool isPayment)
        {
            try
            {
                string saleTitle = Localization.Localization.SaleTo + CustomerName.Trim();

                // Retrieve the active financial year
                var financialYearCheck = await _financialYearRepository.GetSingleActiveAsync();
                string FinancialYearID = financialYearCheck != null ? Convert.ToString(financialYearCheck) : string.Empty;
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return Localization.Localization.CompanyFinancialYearNotSet;
                }

                // Credit Entry Sale
                // 9 - Sale
                var saleAccount = await _accountSettingRepository.GetByActivityAsync(9, CompanyID, BranchID);
                if (saleAccount == null)
                {
                    return Localization.Localization.AccountSettingsForSaleNotFound;
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
                // 10 - Sale Payment Pending
                saleAccount = await _accountSettingRepository.GetByActivityAsync(10, CompanyID, BranchID);
                if (saleAccount == null)
                {
                    return Localization.Localization.AccountSettingsForSalePaymentPendingNotFound;
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
                    CustomerName + ", Sale Payment is Pending!");

                if (isPayment)
                {
                    string payInvoiceNo = "INP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                    // Credit Entry Sale Payment Paid
                    // 11 - Sale Payment Paid
                    saleAccount = await _accountSettingRepository.GetByActivityAsync(11, CompanyID, BranchID);
                    if (saleAccount == null)
                    {
                        return Localization.Localization.AccountSettingsForSalePaymentPaidNotFound;
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
                        "Sale Payment Paid By " + CustomerName);

                    // Debit Entry Sale Payment Success
                    // 12 - Sale Payment Success
                    saleAccount = await _accountSettingRepository.GetByActivityAsync(12, CompanyID, BranchID);
                    if (saleAccount == null)
                    {
                        return Localization.Localization.AccountSettingsForSalePaymentSuccessNotFound;
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
                        CustomerName + ", Sale Payment is Succeed!");

                    // Insert payment record
                    return await _saleRepository.ConfirmSale(CompanyID, BranchID, UserID, CustomerInvoiceID, Amount, CustomerID, payInvoiceNo, 0);
                }

                return await _saleRepository.InsertTransaction(CompanyID, BranchID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                return Localization.Localization.UnexpectedErrorOccurred;
            }
        }

        public async Task<string> SalePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, float TotalAmount, float Amount, string CustomerID, string CustomerName, float RemainingBalance)
        {
            try
            {
                string payInvoiceNo = "INP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                string saleTitle = Localization.Localization.SaleTo + CustomerName.Trim();

                // Retrieve the active financial year
                var financialYearCheck = await _financialYearRepository.GetSingleActiveAsync();
                string FinancialYearID = financialYearCheck != null ? Convert.ToString(financialYearCheck) : string.Empty;
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return Localization.Localization.CompanyFinancialYearNotSet;
                }

                // Credit Entry Sale Payment Paid
                // 11 - Sale Payment Paid
                var saleAccount = await _accountSettingRepository.GetByActivityAsync(11, CompanyID, BranchID);
                if (saleAccount == null)
                {
                    return Localization.Localization.AccountSettingsForSalePaymentPaidNotFound;
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
                    "Sale Payment Paid By " + CustomerName);

                // Debit Entry Sale Payment Success
                // 12 - Sale Payment Success
                saleAccount = await _accountSettingRepository.GetByActivityAsync(12, CompanyID, BranchID);
                if (saleAccount == null)
                {
                    return Localization.Localization.AccountSettingsForSalePaymentSuccessNotFound;
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
                    CustomerName + ", Sale Payment is Succeed!");

                // Insert payment record
                await _saleRepository.ConfirmSale(CompanyID, BranchID, UserID, CustomerInvoiceID, Amount, CustomerID, payInvoiceNo, RemainingBalance);
                // Insert transaction
                await _saleRepository.InsertTransaction(CompanyID, BranchID);

                return Localization.Localization.PaidSuccessfully;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                return Localization.Localization.UnexpectedErrorOccurred;
            }
        }

        // Sale Return
        public async Task<string> ReturnSale(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, int CustomerReturnInvoiceID, float Amount, string CustomerID, string Customername, bool isPayment)
        {
            try
            {
                string returnSaleTitle = Localization.Localization.ReturnSaleFrom + Customername.Trim();

                // Retrieve the active financial year
                var financialYearCheck = await _financialYearRepository.GetSingleActiveAsync();
                string FinancialYearID = financialYearCheck != null ? Convert.ToString(financialYearCheck) : string.Empty;
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return Localization.Localization.CompanyFinancialYearNotSet;
                }

                // Debit Entry Return Sale
                // 13 - Sale Return
                var returnSaleAccount = await _accountSettingRepository.GetByActivityAsync(13, CompanyID, BranchID);
                if (returnSaleAccount == null)
                {
                    return Localization.Localization.AccountSettingsForSaleReturnNotFound;
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
                // 8 - Sale Return Payment Pending
                returnSaleAccount = await _accountSettingRepository.GetByActivityAsync(8, CompanyID, BranchID);
                if (returnSaleAccount == null)
                {
                    return Localization.Localization.AccountSettingsForSaleReturnPaymentPendingNotFound;
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
                    Customername + ", Return Sale Payment is Pending!");

                if (isPayment)
                {
                    string payInvoiceNo = "RIP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                    // Credit Entry Return Sale Payment Paid
                    // 15 - Sale Return Payment Paid
                    returnSaleAccount = await _accountSettingRepository.GetByActivityAsync(15, CompanyID, BranchID);
                    if (returnSaleAccount == null)
                    {
                        return Localization.Localization.AccountSettingsForSaleReturnPaymentPaidNotFound;
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
                        "Return Sale Payment Paid to " + Customername);

                    // Debit Entry Return Sale Payment Success
                    // 16 - Sale Return Payment Succeed
                    returnSaleAccount = await _accountSettingRepository.GetByActivityAsync(16, CompanyID, BranchID);
                    if (returnSaleAccount == null)
                    {
                        return Localization.Localization.AccountSettingsForSalePaymentSuccessNotFound;
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
                        Customername + ", Return Sale Payment is Succeed!");

                    return await _saleRepository.ReturnSale(CompanyID, BranchID, UserID, CustomerInvoiceID, CustomerReturnInvoiceID, Amount, CustomerID, payInvoiceNo, 0);
                }

                return await _saleRepository.InsertTransaction(CompanyID, BranchID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                return Localization.Localization.UnexpectedErrorOccurred;
            }
        }

        public async Task<string> ReturnSalePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, int CustomerReturnInvoiceID, float TotalAmount, float Amount, string CustomerID, string Customername, float RemainingBalance)
        {
                try
                {
                    string saleTitle = Localization.Localization.ReturnSaleFrom + Customername.Trim();
                    string payInvoiceNo = "RIP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                    // Retrieve the active financial year
                    var financialYearCheck = await _financialYearRepository.GetSingleActiveAsync();
                    string FinancialYearID = financialYearCheck != null ? Convert.ToString(financialYearCheck) : string.Empty;
                    if (string.IsNullOrEmpty(FinancialYearID))
                    {
                        return Localization.Localization.CompanyFinancialYearNotSet;
                    }

                    string transactionTitle;

                    // Credit Entry Return Sale Payment Paid
                    // 15 - Sale Return Payment Paid
                    var returnSaleAccount = await _accountSettingRepository.GetByActivityAsync(16, CompanyID, BranchID);
                    if (returnSaleAccount == null)
                    {
                        return Localization.Localization.AccountSettingsForSaleReturnPaymentPaidNotFound;
                    }

                    transactionTitle = Localization.Localization.ReturnSalePaymentPaidTo + Customername;
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
                    // 16 - Sale Return Payment Succeed
                    returnSaleAccount = await _accountSettingRepository.GetByActivityAsync(16, CompanyID, BranchID);
                    if (returnSaleAccount == null)
                    {
                        return Localization.Localization.AccountSettingsForSaleReturnPaymentSuccessNotFound;
                    }
                    transactionTitle = Customername + Localization.Localization.ReturnSalePaymentSsSucceed;
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

                    // Insert return payment record
                    await _saleRepository.ReturnSalePayment(CompanyID, BranchID, UserID, InvoiceNo, CustomerInvoiceID, CustomerReturnInvoiceID, TotalAmount, Amount, CustomerID, RemainingBalance);
                    // Insert transaction records
                    await _saleRepository.InsertTransaction(CompanyID, BranchID);
                    
                    return Localization.Localization.PaidSuccessfully;
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
