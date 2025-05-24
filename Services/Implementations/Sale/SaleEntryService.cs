using Domain.Enums;
using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.Extensions.Logging;

namespace Services.Implementations
{
    public class SaleEntryService : TransactionServiceBase, ISaleEntryService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ISaleCartDetailRepository _saleCartDetailRepository;

        public SaleEntryService(
            ISaleRepository saleRepository,
            IStockRepository stockRepository,
            ISaleCartDetailRepository saleCartDetailRepository,
            IFinancialYearRepository financialYearRepository,
            IAccountSettingRepository accountSettingRepository,
            ILogger<SaleEntryService> logger)
            : base(financialYearRepository, accountSettingRepository, logger)
        {
            _saleRepository = saleRepository ?? throw new ArgumentNullException(nameof(saleRepository));
            _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(stockRepository));
            _saleCartDetailRepository = saleCartDetailRepository ?? throw new ArgumentNullException(nameof(saleCartDetailRepository));
        }

        public async Task<string> ConfirmSale(SaleConfirm saleConfirm, string invoiceNo, string customerInvoiceID, float amount, string customerID, string customerName)
        {
            try
            {
                var financialYearID = await GetFinancialYearID();
                if (string.IsNullOrEmpty(financialYearID))
                    return Localization.CloudERP.Messages.Messages.CompanyFinancialYearNotSet;

                var builder = new TransactionBuilder();
                var saleTitle = $"{Localization.Services.Localization.SaleTo} {customerName.Trim()}";

                // Sale Entry
                await AddTransactionEntry(builder, financialYearID, SaleActivityType.Sale, saleConfirm.CompanyID, saleConfirm.BranchID, saleConfirm.UserID.ToString(), invoiceNo, amount, saleTitle, true);
                // Payment Pending
                await AddTransactionEntry(builder, financialYearID, SaleActivityType.PaymentPending, saleConfirm.CompanyID, saleConfirm.BranchID, saleConfirm.UserID.ToString(), invoiceNo, amount,
                    $"{customerName}, {Localization.Services.Localization.SalePaymentIsPending}", false);

                if (saleConfirm.IsPayment)
                {
                    var payInvoiceNo = GenerateInvoiceNo("INP");
                    // Payment Paid
                    await AddTransactionEntry(builder, financialYearID, SaleActivityType.PaymentPaid, saleConfirm.CompanyID, saleConfirm.BranchID, saleConfirm.UserID.ToString(), payInvoiceNo, amount,
                        $"{Localization.Services.Localization.SalePaymentPaidBy} {customerName}", true);
                    // Payment Success
                    await AddTransactionEntry(builder, financialYearID, SaleActivityType.PaymentSuccess, saleConfirm.CompanyID, saleConfirm.BranchID, saleConfirm.UserID.ToString(), payInvoiceNo, amount,
                        $"{customerName}, {Localization.Services.Localization.SalePaymentIsSucceed}", false);

                    await _saleRepository.SetEntries(ConvertToDataTable(builder.Build()));
                    return await _saleRepository.ConfirmSale(saleConfirm.CompanyID, saleConfirm.BranchID, saleConfirm.UserID, customerInvoiceID, amount, customerID, payInvoiceNo, 0);
                }

                await _saleRepository.SetEntries(ConvertToDataTable(builder.Build()));
                return await _saleRepository.InsertTransaction(saleConfirm.CompanyID, saleConfirm.BranchID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ConfirmSale");
                return Localization.Services.Localization.UnexpectedErrorOccurred;
            }
        }

        public async Task<string> SalePayment(int companyID, int branchID, int userID, string invoiceNo, string customerInvoiceID, float totalAmount, float amount, string customerID, string customerName, float remainingBalance)
        {
            try
            {
                var financialYearID = await GetFinancialYearID();
                if (string.IsNullOrEmpty(financialYearID))
                    return Localization.CloudERP.Messages.Messages.CompanyFinancialYearNotSet;

                var builder = new TransactionBuilder();
                var payInvoiceNo = GenerateInvoiceNo("INP");

                // Payment Paid
                await AddTransactionEntry(builder, financialYearID, SaleActivityType.PaymentPaid, companyID, branchID, userID.ToString(), invoiceNo, amount,
                    $"{Localization.Services.Localization.SalePaymentPaidBy} {customerName}", true);
                // Payment Success
                await AddTransactionEntry(builder, financialYearID, SaleActivityType.PaymentSuccess, companyID, branchID, userID.ToString(), invoiceNo, amount,
                    $"{customerName}, {Localization.Services.Localization.SalePaymentIsSucceed}", false);

                await _saleRepository.SetEntries(ConvertToDataTable(builder.Build()));
                await _saleRepository.ConfirmSale(companyID, branchID, userID, customerInvoiceID, amount, customerID, payInvoiceNo, remainingBalance);
                await _saleRepository.InsertTransaction(companyID, branchID);

                return Localization.Services.Localization.PaidSuccessfully;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SalePayment");
                return Localization.Services.Localization.UnexpectedErrorOccurred;
            }
        }

        public async Task<string> ReturnSale(int companyID, int branchID, int userID, string invoiceNo, string customerInvoiceID, int customerReturnInvoiceID, float amount, string customerID, string customerName, bool isPayment)
        {
            try
            {
                var financialYearID = await GetFinancialYearID();
                if (string.IsNullOrEmpty(financialYearID))
                    return Localization.CloudERP.Messages.Messages.CompanyFinancialYearNotSet;

                var builder = new TransactionBuilder();
                var returnSaleTitle = $"{Localization.Services.Localization.ReturnSaleFrom} {customerName.Trim()}";

                // Return Sale
                await AddTransactionEntry(builder, financialYearID, SaleActivityType.Return, companyID, branchID, userID.ToString(), invoiceNo, amount, returnSaleTitle, false);
                // Return Payment Pending
                await AddTransactionEntry(builder, financialYearID, SaleActivityType.ReturnPaymentPending, companyID, branchID, userID.ToString(), invoiceNo, amount,
                    $"{customerName}, {Localization.Services.Localization.ReturnSalePaymentIsPending}", true);

                if (isPayment)
                {
                    var payInvoiceNo = GenerateInvoiceNo("RIP");
                    // Return Payment Paid
                    await AddTransactionEntry(builder, financialYearID, SaleActivityType.ReturnPaymentPaid, companyID, branchID, userID.ToString(), payInvoiceNo, amount,
                        $"{Localization.Services.Localization.ReturnSalePaymentPaidTo} {customerName}", false);
                    // Return Payment Success
                    await AddTransactionEntry(builder, financialYearID, SaleActivityType.ReturnPaymentSuccess, companyID, branchID, userID.ToString(), payInvoiceNo, amount,
                        $"{customerName}, {Localization.Services.Localization.ReturnSalePaymentIsSucceed}", false);

                    await _saleRepository.SetEntries(ConvertToDataTable(builder.Build()));
                    return await _saleRepository.ReturnSale(companyID, branchID, userID, customerInvoiceID, customerReturnInvoiceID, amount, customerID, payInvoiceNo, 0);
                }

                await _saleRepository.SetEntries(ConvertToDataTable(builder.Build()));
                return await _saleRepository.InsertTransaction(companyID, branchID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReturnSale");
                return Localization.Services.Localization.UnexpectedErrorOccurred;
            }
        }

        public async Task<string> ReturnSalePayment(int companyID, int branchID, int userID, string invoiceNo, string customerInvoiceID, int customerReturnInvoiceID, float totalAmount, float amount, string customerID, string customerName, float remainingBalance)
        {
            try
            {
                var financialYearID = await GetFinancialYearID();
                if (string.IsNullOrEmpty(financialYearID))
                    return Localization.CloudERP.Messages.Messages.CompanyFinancialYearNotSet;

                var builder = new TransactionBuilder();
                var payInvoiceNo = GenerateInvoiceNo("RIP");

                // Return Payment Paid
                await AddTransactionEntry(builder, financialYearID, SaleActivityType.ReturnPaymentPaid, companyID, branchID, userID.ToString(), invoiceNo, amount,
                    $"{Localization.Services.Localization.ReturnSalePaymentPaidTo} {customerName}", false);
                // Return Payment Success
                await AddTransactionEntry(builder, financialYearID, SaleActivityType.ReturnPaymentSuccess, companyID, branchID, userID.ToString(), invoiceNo, amount,
                    $"{customerName}, {Localization.Services.Localization.ReturnSalePaymentIsSucceed}", true);

                await _saleRepository.SetEntries(ConvertToDataTable(builder.Build()));
                await _saleRepository.ReturnSalePayment(companyID, branchID, userID, invoiceNo, customerInvoiceID, customerReturnInvoiceID, totalAmount, amount, customerID, remainingBalance);
                await _saleRepository.InsertTransaction(companyID, branchID);

                return Localization.Services.Localization.PaidSuccessfully;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReturnSalePayment");
                return Localization.Services.Localization.UnexpectedErrorOccurred;
            }
        }

        public async Task CompleteSale(IEnumerable<SaleCartDetail> saleDetails)
        {
            foreach (var item in saleDetails)
            {
                var stockItem = await _stockRepository.GetByIdAsync(item.ProductID);
                if (stockItem != null)
                {
                    stockItem.Quantity += item.SaleQuantity;
                    await _stockRepository.UpdateAsync(stockItem);
                }
                await _saleCartDetailRepository.UpdateAsync(item);
            }
        }
    }
}