using Domain.RepositoryAccess;
using Domain.Services;

namespace CloudERP.Facades
{
    public class PurchasePaymentReturnFacade
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ISupplierReturnPaymentRepository _supplierReturnPaymentRepository;
        private readonly ISupplierReturnInvoiceRepository _supplierReturnInvoiceRepository;
        private readonly IPurchaseEntryService _purchaseEntry;

        public PurchasePaymentReturnFacade(
            IPurchaseRepository purchaseRepository,
            ISupplierRepository supplierRepository,
            ISupplierReturnPaymentRepository supplierReturnPaymentRepository,
            ISupplierReturnInvoiceRepository supplierReturnInvoiceRepository,
            IPurchaseEntryService purchaseEntry)
        {
            _purchaseRepository = purchaseRepository;
            _supplierRepository = supplierRepository;
            _supplierReturnPaymentRepository = supplierReturnPaymentRepository;
            _supplierReturnInvoiceRepository = supplierReturnInvoiceRepository;
            _purchaseEntry = purchaseEntry;
        }

        public IPurchaseRepository PurchaseRepository => _purchaseRepository;
        public ISupplierRepository SupplierRepository => _supplierRepository;
        public ISupplierReturnPaymentRepository SupplierReturnPaymentRepository => _supplierReturnPaymentRepository;
        public ISupplierReturnInvoiceRepository SupplierReturnInvoiceRepository => _supplierReturnInvoiceRepository;
        public IPurchaseEntryService PurchaseEntry => _purchaseEntry;
    }
}