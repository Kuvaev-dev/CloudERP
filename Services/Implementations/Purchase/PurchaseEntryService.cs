using Domain.Enums;
using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.Extensions.Logging;

namespace Services.Implementations
{
    public class PurchaseEntryService : TransactionServiceBase, IPurchaseEntryService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IPurchaseCartDetailRepository _purchaseCartDetailRepository;

        public PurchaseEntryService(
            IPurchaseRepository purchaseRepository,
            IStockRepository stockRepository,
            IPurchaseCartDetailRepository purchaseCartDetailRepository,
            IFinancialYearRepository financialYearRepository,
            IAccountSettingRepository accountSettingRepository,
            ILogger<PurchaseEntryService> logger)
            : base(financialYearRepository, accountSettingRepository, logger)
        {
            _purchaseRepository = purchaseRepository ?? throw new ArgumentNullException(nameof(purchaseRepository));
            _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(stockRepository));
            _purchaseCartDetailRepository = purchaseCartDetailRepository ?? throw new ArgumentNullException(nameof(purchaseCartDetailRepository));
        }

        public async Task<string> ConfirmPurchase(int companyID, int branchID, int userID, string invoiceNo, string supplierInvoiceID, float amount, string supplierID, string supplierName, bool isPayment)
        {
            try
            {
                var financialYearID = await GetFinancialYearID();
                if (string.IsNullOrEmpty(financialYearID))
                    return Localization.CloudERP.Messages.Messages.CompanyFinancialYearNotSet;

                var builder = new TransactionBuilder();
                var purchaseTitle = $"{Localization.Services.Localization.PurchaseFrom} {supplierName.Trim()}";

                // Purchase Entry
                await AddTransactionEntry(builder, financialYearID, PurchaseActivityType.Purchase, companyID, branchID, userID.ToString(), invoiceNo, amount, purchaseTitle, false);
                // Payment Pending
                await AddTransactionEntry(builder, financialYearID, PurchaseActivityType.PaymentPending, companyID, branchID, userID.ToString(), invoiceNo, amount,
                    $"{supplierName}, {Localization.Services.Localization.ReturnPurchasePaymentIsPending}", true);

                if (isPayment)
                {
                    var payInvoiceNo = GenerateInvoiceNo("PAY");
                    // Payment Paid
                    await AddTransactionEntry(builder, financialYearID, PurchaseActivityType.PaymentPaid, companyID, branchID, userID.ToString(), payInvoiceNo, amount,
                        $"{Localization.Services.Localization.PaymentPaidTo} {supplierName}", false);
                    // Payment Success
                    await AddTransactionEntry(builder, financialYearID, PurchaseActivityType.PaymentSuccess, companyID, branchID, userID.ToString(), payInvoiceNo, amount,
                        $"{supplierName}, {Localization.Services.Localization.PurchasePaymentIsSucceed}", true);

                    await _purchaseRepository.SetEntries(ConvertToDataTable(builder.Build()));
                    return await _purchaseRepository.ConfirmPurchase(companyID, branchID, userID, supplierInvoiceID, amount, supplierID, payInvoiceNo, 0);
                }

                await _purchaseRepository.SetEntries(ConvertToDataTable(builder.Build()));
                return await _purchaseRepository.InsertTransaction(companyID, branchID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ConfirmPurchase");
                return Localization.Services.Localization.UnexpectedErrorOccurred;
            }
        }

        public async Task<string> PurchasePayment(int companyID, int branchID, int userID, string invoiceNo, string supplierInvoiceID, float totalAmount, float amount, string supplierID, string supplierName, float remainingBalance)
        {
            try
            {
                var financialYearID = await GetFinancialYearID();
                if (string.IsNullOrEmpty(financialYearID))
                    return Localization.CloudERP.Messages.Messages.CompanyFinancialYearNotSet;

                var builder = new TransactionBuilder();
                var purchaseTitle = $"{Localization.Services.Localization.PurchaseFrom} {supplierName.Trim()}";
                var payInvoiceNo = GenerateInvoiceNo("PAY");

                // Purchase Entry
                await AddTransactionEntry(builder, financialYearID, PurchaseActivityType.Purchase, companyID, branchID, userID.ToString(), invoiceNo, amount, purchaseTitle, false);
                // Payment Pending
                await AddTransactionEntry(builder, financialYearID, PurchaseActivityType.PaymentPending, companyID, branchID, userID.ToString(), invoiceNo, amount,
                    $"{supplierName}, {Localization.Services.Localization.ReturnPurchasePaymentIsPending}", true);

                if (amount > 0)
                {
                    // Payment Paid
                    await AddTransactionEntry(builder, financialYearID, PurchaseActivityType.PaymentPaid, companyID, branchID, userID.ToString(), payInvoiceNo, amount,
                        $"{Localization.Services.Localization.PaymentPaidTo} {supplierName}", false);
                    // Payment Success
                    await AddTransactionEntry(builder, financialYearID, PurchaseActivityType.PaymentSuccess, companyID, branchID, userID.ToString(), payInvoiceNo, amount,
                        $"{supplierName}, {Localization.Services.Localization.PurchasePaymentIsSucceed}", true);

                    await _purchaseRepository.SetEntries(ConvertToDataTable(builder.Build()));
                    return await _purchaseRepository.ConfirmPurchase(companyID, branchID, userID, supplierInvoiceID, amount, supplierID, payInvoiceNo, remainingBalance);
                }

                await _purchaseRepository.SetEntries(ConvertToDataTable(builder.Build()));
                return await _purchaseRepository.InsertTransaction(companyID, branchID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in PurchasePayment");
                return Localization.Services.Localization.UnexpectedErrorOccurred;
            }
        }

        public async Task<string> ReturnPurchase(int companyID, int branchID, int userID, string invoiceNo, string supplierInvoiceID, int supplierReturnInvoiceID, float amount, string supplierID, string supplierName, bool isPayment)
        {
            try
            {
                var financialYearID = await GetFinancialYearID();
                if (string.IsNullOrEmpty(financialYearID))
                    return Localization.CloudERP.Messages.Messages.CompanyFinancialYearNotSet;

                var builder = new TransactionBuilder();
                var returnPurchaseTitle = $"{Localization.Services.Localization.ReturnPurchaseTo} {supplierName.Trim()}";

                // Return Purchase
                await AddTransactionEntry(builder, financialYearID, PurchaseActivityType.Return, companyID, branchID, userID.ToString(), invoiceNo, amount, returnPurchaseTitle, true);
                // Return Payment Pending
                await AddTransactionEntry(builder, financialYearID, PurchaseActivityType.ReturnPaymentPending, companyID, branchID, userID.ToString(), invoiceNo, amount,
                    $"{supplierName}, {Localization.Services.Localization.ReturnPurchasePaymentIsSucceed}", false);

                var successMessage = Localization.Services.Localization.ReturnPurchaseSuccess;

                if (isPayment)
                {
                    var payInvoiceNo = GenerateInvoiceNo("RPP");
                    // Return Payment Pending
                    await AddTransactionEntry(builder, financialYearID, PurchaseActivityType.ReturnPaymentPending, companyID, branchID, userID.ToString(), payInvoiceNo, amount,
                        $"{Localization.Services.Localization.ReturnPaymentFrom} {supplierName}", true);
                    // Return Payment Success
                    await AddTransactionEntry(builder, financialYearID, PurchaseActivityType.ReturnPaymentSuccess, companyID, branchID, userID.ToString(), payInvoiceNo, amount,
                        $"{supplierName}, {Localization.Services.Localization.ReturnPurchasePaymentIsSucceed}", false);

                    await _purchaseRepository.SetEntries(ConvertToDataTable(builder.Build()));
                    successMessage += await _purchaseRepository.ConfirmPurchaseReturn(companyID, branchID, userID, supplierInvoiceID, supplierReturnInvoiceID, amount, supplierID, payInvoiceNo);
                }

                await _purchaseRepository.SetEntries(ConvertToDataTable(builder.Build()));
                await _purchaseRepository.InsertTransaction(companyID, branchID);
                return successMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReturnPurchase");
                return Localization.Services.Localization.UnexpectedErrorOccurred;
            }
        }

        public async Task<string> ReturnPurchasePayment(int companyID, int branchID, int userID, string invoiceNo, string supplierInvoiceID, int supplierReturnInvoiceID, float totalAmount, float amount, string supplierID, string supplierName, float remainingBalance)
        {
            try
            {
                var financialYearID = await GetFinancialYearID();
                if (string.IsNullOrEmpty(financialYearID))
                    return Localization.CloudERP.Messages.Messages.CompanyFinancialYearNotSet;

                var builder = new TransactionBuilder();
                var payInvoiceNo = GenerateInvoiceNo("RPP");

                // Return Payment Pending
                await AddTransactionEntry(builder, financialYearID, PurchaseActivityType.ReturnPaymentPending, companyID, branchID, userID.ToString(), invoiceNo, amount,
                    $"{Localization.Services.Localization.ReturnPaymentFrom} {supplierName}", true);
                // Return Payment Success
                await AddTransactionEntry(builder, financialYearID, PurchaseActivityType.ReturnPaymentSuccess, companyID, branchID, userID.ToString(), invoiceNo, amount,
                    $"{supplierName}, {Localization.Services.Localization.ReturnPurchasePaymentIsSucceed}", false);

                await _purchaseRepository.SetEntries(ConvertToDataTable(builder.Build()));
                await _purchaseRepository.ConfirmPurchaseReturn(companyID, branchID, userID, supplierInvoiceID, supplierReturnInvoiceID, amount, supplierID, payInvoiceNo);
                await _purchaseRepository.InsertTransaction(companyID, branchID);

                return Localization.Services.Localization.ReturnPurchasePaymentIsSucceed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReturnPurchasePayment");
                return Localization.Services.Localization.UnexpectedErrorOccurred;
            }
        }

        public async Task CompletePurchase(IEnumerable<PurchaseCartDetail> purchaseDetails)
        {
            foreach (var item in purchaseDetails)
            {
                var stockItem = await _stockRepository.GetByIdAsync(item.ProductID);
                if (stockItem != null)
                {
                    stockItem.Quantity += item.PurchaseQuantity;
                    await _stockRepository.UpdateAsync(stockItem);
                }
                await _purchaseCartDetailRepository.UpdateAsync(item);
            }
        }
    }
}