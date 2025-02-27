using Domain.Facades;
using Domain.Models;
using Domain.ServiceAccess;

namespace Services.Implementations
{
    public class PurchasePaymentService : IPurchasePaymentService
    {
        private readonly PurchasePaymentFacade _purchasePaymentFacade;

        public PurchasePaymentService(PurchasePaymentFacade purchasePaymentFacade)
        {
            _purchasePaymentFacade = purchasePaymentFacade ?? throw new ArgumentNullException(nameof(PurchasePaymentFacade));
        }

        public async Task<(IEnumerable<object> PaymentHistory, IEnumerable<object> ReturnDetails, double RemainingAmount)> GetPaymentDetailsAsync(int invoiceId)
        {
            var paymentHistory = await _purchasePaymentFacade.PurchaseRepository.PurchasePaymentHistory(invoiceId);
            var returnDetails = await _purchasePaymentFacade.SupplierReturnInvoiceRepository.GetReturnDetails(invoiceId);

            double? totalInvoiceAmount = await _purchasePaymentFacade.SupplierInvoiceRepository.GetTotalAmountAsync(invoiceId);
            double totalPaidAmount = await _purchasePaymentFacade.SupplierPaymentRepository.GetTotalPaidAmount(invoiceId);
            double remainingAmount = totalInvoiceAmount.Value - totalPaidAmount;

            return (paymentHistory, returnDetails, remainingAmount);
        }

        public async Task<List<PurchaseInfo>> GetPurchasePaymentHistoryAsync(int invoiceId)
        {
            return await _purchasePaymentFacade.PurchaseRepository.PurchasePaymentHistory(invoiceId);
        }

        public async Task<double?> GetTotalAmountByIdAsync(int invoiceId)
        {
            return await _purchasePaymentFacade.SupplierInvoiceRepository.GetTotalAmountAsync(invoiceId);
        }

        public async Task<double> GetTotalPaidAmountByIdAsync(int invoiceId)
        {
            return await _purchasePaymentFacade.SupplierPaymentRepository.GetTotalPaidAmount(invoiceId);
        }

        public async Task<string> ProcessPaymentAsync(int companyId, int branchId, int userId, PurchaseAmount paymentDto)
        {
            if (paymentDto.PaidAmount > paymentDto.PreviousRemainingAmount)
            {
                return "Purchase Payment Remaining Amount Error";
            }

            string paymentInvoiceNo = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
            var purchaseInvoice = await _purchasePaymentFacade.SupplierInvoiceRepository.GetByIdAsync(paymentDto.InvoiceId);
            var supplier = await _purchasePaymentFacade.SupplierRepository.GetByIdAsync(purchaseInvoice.SupplierID);

            return await _purchasePaymentFacade.PurchaseEntryService.PurchasePayment(
                companyId,
                branchId,
                userId,
                paymentInvoiceNo,
                Convert.ToString(paymentDto.InvoiceId),
                (float)purchaseInvoice.TotalAmount,
                paymentDto.PaidAmount,
                Convert.ToString(supplier.SupplierID),
                supplier.SupplierName,
                paymentDto.PreviousRemainingAmount - paymentDto.PaidAmount
            );
        }
    }
}
