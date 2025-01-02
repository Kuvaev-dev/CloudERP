using Domain.Facades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IPurchasePaymentService
    {
        Task<(IEnumerable<object> PaymentHistory, IEnumerable<object> ReturnDetails, double RemainingAmount)> GetPaymentDetailsAsync(int invoiceId);
        Task<string> ProcessPaymentAsync(int companyId, int branchId, int userId, int invoiceId, float previousRemainingAmount, float paymentAmount);
    }

    public class PurchasePaymentService : IPurchasePaymentService
    {
        private readonly PurchasePaymentFacade _purchasePaymentFacade;

        public PurchasePaymentService(PurchasePaymentFacade purchasePaymentFacade)
        {
            _purchasePaymentFacade = purchasePaymentFacade;
        }

        public async Task<(IEnumerable<object> PaymentHistory, IEnumerable<object> ReturnDetails, double RemainingAmount)> GetPaymentDetailsAsync(int invoiceId)
        {
            var paymentHistory = await _purchasePaymentFacade.PurchaseRepository.PurchasePaymentHistory(invoiceId);
            var returnDetails = await _purchasePaymentFacade.SupplierReturnInvoiceRepository.GetReturnDetails(invoiceId);

            double totalInvoiceAmount = await _purchasePaymentFacade.SupplierInvoiceRepository.GetTotalAmountAsync(invoiceId);
            double totalPaidAmount = await _purchasePaymentFacade.SupplierPaymentRepository.GetTotalPaidAmount(invoiceId);
            double remainingAmount = totalInvoiceAmount - totalPaidAmount;

            return (paymentHistory, returnDetails, remainingAmount);
        }

        public async Task<string> ProcessPaymentAsync(int companyId, int branchId, int userId, int invoiceId, float previousRemainingAmount, float paymentAmount)
        {
            if (paymentAmount > previousRemainingAmount)
            {
                return "Purchase Payment Remaining Amount Error";
            }

            string paymentInvoiceNo = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
            int supplierId = await _purchasePaymentFacade.SupplierInvoiceRepository.GetSupplierIdFromInvoice(invoiceId);
            var supplier = await _purchasePaymentFacade.SupplierRepository.GetByIdAsync(supplierId);
            var purchaseInvoice = await _purchasePaymentFacade.SupplierInvoiceRepository.GetByIdAsync(invoiceId);

            return await _purchasePaymentFacade.PurchaseEntryService.PurchasePayment(
                companyId,
                branchId,
                userId,
                paymentInvoiceNo,
                invoiceId.ToString(),
                (float)purchaseInvoice.TotalAmount,
                paymentAmount,
                supplierId.ToString(),
                supplier?.SupplierName,
                previousRemainingAmount - paymentAmount
            );
        }
    }
}
