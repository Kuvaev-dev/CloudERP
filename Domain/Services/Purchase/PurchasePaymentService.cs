using Domain.Facades;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Services.Purchase
{
    public interface IPurchasePaymentService
    {
        Task<(IEnumerable<object> PaymentHistory, IEnumerable<object> ReturnDetails, double RemainingAmount)> GetPaymentDetailsAsync(int invoiceId);
        Task<string> ProcessPaymentAsync(int companyId, int branchId, int userId, PurchasePayment paymentDto);
        Task<List<PurchasePaymentModel>> GetPurchasePaymentHistoryAsync(int invoiceId);
        Task<double> GetTotalAmountByIdAsync(int invoiceId);
        Task<double> GetTotalPaidAmountByIdAsync(int invoiceId);
    }

    public class PurchasePaymentService : IPurchasePaymentService
    {
        private readonly PurchasePaymentFacade _purchasePaymentFacade;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ISupplierInvoiceRepository _supplierInvoiceRepository;
        private readonly ISupplierPaymentRepository _supplierPaymentRepository;

        public PurchasePaymentService(
            PurchasePaymentFacade purchasePaymentFacade, 
            IPurchaseRepository purchaseRepository,
            ISupplierInvoiceRepository supplierInvoiceRepository,
            ISupplierPaymentRepository supplierPaymentRepository)
        {
            _purchasePaymentFacade = purchasePaymentFacade ?? throw new ArgumentNullException(nameof(PurchasePaymentFacade));
            _purchaseRepository = purchaseRepository;
            _supplierInvoiceRepository = supplierInvoiceRepository;
            _supplierPaymentRepository = supplierPaymentRepository;
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

        public async Task<List<PurchasePaymentModel>> GetPurchasePaymentHistoryAsync(int invoiceId)
        {
            return await _purchaseRepository.PurchasePaymentHistory(invoiceId);
        }

        public async Task<double> GetTotalAmountByIdAsync(int invoiceId)
        {
            return await _supplierInvoiceRepository.GetTotalAmountAsync(invoiceId);
        }

        public async Task<double> GetTotalPaidAmountByIdAsync(int invoiceId)
        {
            return await _supplierPaymentRepository.GetTotalPaidAmount(invoiceId);
        }

        public async Task<string> ProcessPaymentAsync(int companyId, int branchId, int userId, PurchasePayment paymentDto)
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
