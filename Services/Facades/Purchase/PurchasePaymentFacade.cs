using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace Domain.Facades
{
    public class PurchasePaymentFacade
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ISupplierReturnInvoiceRepository _supplierReturnInvoiceRepository;
        private readonly ISupplierInvoiceRepository _supplierInvoiceRepository;
        private readonly ISupplierInvoiceDetailRepository _supplierInvoiceDetailRepository;
        private readonly ISupplierPaymentRepository _supplierPaymentRepository;
        private readonly IPurchaseEntryService _purchaseEntryService;
        private readonly IPurchasePaymentService _purchasePaymentService;
        private readonly IPurchaseService _purchaseService;

        public PurchasePaymentFacade(
            IPurchaseRepository purchaseRepository,
            ISupplierRepository supplierRepository,
            ISupplierReturnInvoiceRepository supplierReturnInvoiceRepository,
            ISupplierInvoiceRepository supplierInvoiceRepository,
            ISupplierPaymentRepository supplierPaymentRepository,
            IPurchaseEntryService purchaseEntryService,
            IPurchaseService purchaseService,
            ISupplierInvoiceDetailRepository supplierInvoiceDetailRepository,
            IPurchasePaymentService purchasePaymentService)
        {
            _purchaseRepository = purchaseRepository ?? throw new ArgumentNullException(nameof(purchaseRepository));
            _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
            _supplierReturnInvoiceRepository = supplierReturnInvoiceRepository ?? throw new ArgumentNullException(nameof(supplierReturnInvoiceRepository));
            _supplierInvoiceRepository = supplierInvoiceRepository ?? throw new ArgumentNullException(nameof(supplierInvoiceRepository));
            _supplierPaymentRepository = supplierPaymentRepository ?? throw new ArgumentNullException(nameof(supplierPaymentRepository));
            _purchaseEntryService = purchaseEntryService ?? throw new ArgumentNullException(nameof(purchaseEntryService));
            _purchaseService = purchaseService ?? throw new ArgumentNullException(nameof(purchaseService));
            _supplierInvoiceDetailRepository = supplierInvoiceDetailRepository ?? throw new ArgumentNullException(nameof(supplierInvoiceDetailRepository));
            _purchasePaymentService = purchasePaymentService;
        }

        public IPurchaseRepository PurchaseRepository => _purchaseRepository;
        public ISupplierRepository SupplierRepository => _supplierRepository;
        public ISupplierReturnInvoiceRepository SupplierReturnInvoiceRepository => _supplierReturnInvoiceRepository;
        public ISupplierInvoiceRepository SupplierInvoiceRepository => _supplierInvoiceRepository;
        public ISupplierPaymentRepository SupplierPaymentRepository => _supplierPaymentRepository;
        public IPurchaseEntryService PurchaseEntryService => _purchaseEntryService;
        public IPurchaseService PurchaseService => _purchaseService;
        public ISupplierInvoiceDetailRepository SupplierInvoiceDetailRepository => _supplierInvoiceDetailRepository;
        public IPurchasePaymentService PurchasePaymentService => _purchasePaymentService;
    }
}
