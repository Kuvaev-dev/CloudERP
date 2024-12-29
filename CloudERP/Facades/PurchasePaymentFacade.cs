using Domain.EntryAccess;
using Domain.RepositoryAccess;

namespace CloudERP.Facades
{
    public class PurchasePaymentFacade
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ISupplierInvoiceRepository _supplierInvoiceRepository;
        private readonly ISupplierPaymentRepository _supplierPaymentRepository;
        private readonly ISupplierReturnInvoiceRepository _supplierReturnInvoiceRepository;
        private readonly ISupplierInvoiceDetailRepository _supplierInvoiceDetailRepository;
        private readonly IPurchaseEntry _paymentEntry;

        public PurchasePaymentFacade(
            IPurchaseRepository purchaseRepository,
            ISupplierRepository supplierRepository,
            ISupplierInvoiceRepository supplierInvoiceRepository,
            ISupplierPaymentRepository supplierPaymentRepository,
            ISupplierReturnInvoiceRepository supplierReturnInvoiceRepository,
            ISupplierInvoiceDetailRepository supplierInvoiceDetailRepository,
            IPurchaseEntry purchaseEntry)
        {
            _purchaseRepository = purchaseRepository;
            _supplierRepository = supplierRepository;
            _supplierInvoiceRepository = supplierInvoiceRepository;
            _supplierPaymentRepository = supplierPaymentRepository;
            _supplierReturnInvoiceRepository = supplierReturnInvoiceRepository;
            _supplierInvoiceDetailRepository = supplierInvoiceDetailRepository;
            _paymentEntry = purchaseEntry;
        }

        public IPurchaseRepository PurchaseRepository => _purchaseRepository;
        public ISupplierRepository SupplierRepository => _supplierRepository;
        public ISupplierInvoiceRepository SupplierInvoiceRepository => _supplierInvoiceRepository;
        public ISupplierPaymentRepository SupplierPaymentRepository => _supplierPaymentRepository;
        public ISupplierReturnInvoiceRepository SupplierReturnInvoiceRepository => _supplierReturnInvoiceRepository;
        public ISupplierInvoiceDetailRepository SupplierInvoiceDetailRepository => _supplierInvoiceDetailRepository;
        public IPurchaseEntry PurchaseEntry => _paymentEntry;
    }
}