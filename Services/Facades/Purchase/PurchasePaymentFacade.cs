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
        private readonly IPurchaseService _purchaseService;

        public PurchasePaymentFacade(
            IPurchaseRepository purchaseRepository,
            ISupplierRepository supplierRepository,
            ISupplierReturnInvoiceRepository supplierReturnInvoiceRepository,
            ISupplierInvoiceRepository supplierInvoiceRepository,
            ISupplierPaymentRepository supplierPaymentRepository,
            IPurchaseEntryService purchaseEntryService,
            IPurchaseService purchaseService,
            ISupplierInvoiceDetailRepository supplierInvoiceDetailRepository)
        {
            _purchaseRepository = purchaseRepository ?? throw new ArgumentNullException(nameof(IPurchaseRepository));
            _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(ISupplierRepository));
            _supplierReturnInvoiceRepository = supplierReturnInvoiceRepository ?? throw new ArgumentNullException(nameof(ISupplierReturnInvoiceRepository));
            _supplierInvoiceRepository = supplierInvoiceRepository ?? throw new ArgumentNullException(nameof(ISupplierInvoiceRepository));
            _supplierPaymentRepository = supplierPaymentRepository ?? throw new ArgumentNullException(nameof(ISupplierPaymentRepository));
            _purchaseEntryService = purchaseEntryService ?? throw new ArgumentNullException(nameof(IPurchaseEntryService));
            _purchaseService = purchaseService ?? throw new ArgumentNullException(nameof(purchaseService));
            _supplierInvoiceDetailRepository = supplierInvoiceDetailRepository ?? throw new ArgumentNullException(nameof(supplierInvoiceDetailRepository));
        }

        public IPurchaseRepository PurchaseRepository => _purchaseRepository;
        public ISupplierRepository SupplierRepository => _supplierRepository;
        public ISupplierReturnInvoiceRepository SupplierReturnInvoiceRepository => _supplierReturnInvoiceRepository;
        public ISupplierInvoiceRepository SupplierInvoiceRepository => _supplierInvoiceRepository;
        public ISupplierPaymentRepository SupplierPaymentRepository => _supplierPaymentRepository;
        public IPurchaseEntryService PurchaseEntryService => _purchaseEntryService;
        public IPurchaseService PurchaseService => _purchaseService;
        public ISupplierInvoiceDetailRepository SupplierInvoiceDetailRepository => _supplierInvoiceDetailRepository;
    }
}
