﻿using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace Services.Implementations
{
    public class PurchasePaymentReturnService : IPurchasePaymentReturnService
    {
        private readonly ISupplierReturnPaymentRepository _supplierReturnPaymentRepository;
        private readonly ISupplierReturnInvoiceRepository _supplierReturnInvoiceRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IPurchaseEntryService _purchaseEntryService;

        public PurchasePaymentReturnService(
            ISupplierReturnPaymentRepository supplierReturnPaymentRepository,
            ISupplierReturnInvoiceRepository supplierReturnInvoiceRepository,
            ISupplierRepository supplierRepository,
            IPurchaseEntryService purchaseEntryService)
        {
            _supplierReturnPaymentRepository = supplierReturnPaymentRepository ?? throw new ArgumentNullException(nameof(ISupplierReturnPaymentRepository));
            _supplierReturnInvoiceRepository = supplierReturnInvoiceRepository ?? throw new ArgumentNullException(nameof(ISupplierReturnInvoiceRepository));
            _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(ISupplierRepository));
            _purchaseEntryService = purchaseEntryService ?? throw new ArgumentNullException(nameof(IPurchaseEntryService));
        }

        public async Task<double> GetRemainingAmountAsync(int? invoiceId)
        {
            var list = await _supplierReturnPaymentRepository.GetBySupplierReturnInvoiceId(invoiceId.Value);
            double? remainingAmount = list.Sum(item => item.RemainingBalance);

            if (remainingAmount == 0)
            {
                remainingAmount = await _supplierReturnInvoiceRepository.GetTotalAmount(invoiceId.Value);
            }

            return remainingAmount.Value;
        }

        public async Task<string> ProcessReturnPaymentAsync(PurchaseReturn returnAmountDto, int branchId, int companyId, int userId)
        {
            if (returnAmountDto.PaymentAmount > returnAmountDto.PreviousRemainingAmount)
            {
                return Localization.Services.Localization.PurchasePaymentRemainingAmountError;
            }

            string payInvoiceNo = "RPP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
            var supplierID = await _supplierReturnInvoiceRepository.GetSupplierIdByInvoice(returnAmountDto.InvoiceId);
            var supplier = await _supplierRepository.GetByIdAsync(supplierID.Value);
            var purchaseInvoice = await _supplierReturnInvoiceRepository.GetById(returnAmountDto.InvoiceId);

            return await _purchaseEntryService.ReturnPurchasePayment(
                companyId,
                branchId,
                userId,
                payInvoiceNo,
                purchaseInvoice.SupplierInvoiceID.ToString(),
                purchaseInvoice.SupplierReturnInvoiceID,
                (float)purchaseInvoice.TotalAmount,
                returnAmountDto.PaymentAmount,
                Convert.ToString(supplier?.SupplierID),
                supplier?.SupplierName,
                returnAmountDto.PreviousRemainingAmount - returnAmountDto.PaymentAmount);
        }
    }
}
