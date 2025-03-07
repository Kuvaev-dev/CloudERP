using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace Domain.Facades
{
    public class PurchaseReturnFacade
    {
        private readonly ISupplierInvoiceDetailRepository _supplierInvoiceDetailRepository;
        private readonly ISupplierInvoiceRepository _supplierInvoiceRepository;
        private readonly ISupplierReturnInvoiceRepository _supplierReturnInvoiceRepository;
        private readonly ISupplierReturnInvoiceDetailRepository _supplierReturnInvoiceDetailRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IPurchaseEntryService _purchaseEntryService;

        public PurchaseReturnFacade(
            ISupplierInvoiceDetailRepository supplierInvoiceDetailRepository,
            ISupplierInvoiceRepository supplierInvoiceRepository,
            ISupplierReturnInvoiceRepository supplierReturnInvoiceRepository,
            ISupplierReturnInvoiceDetailRepository supplierReturnInvoiceDetailRepository,
            ISupplierRepository supplierRepository,
            IStockRepository stockRepository,
            IPurchaseEntryService purchaseEntryService)
        {
            _supplierInvoiceDetailRepository = supplierInvoiceDetailRepository ?? throw new ArgumentNullException(nameof(supplierInvoiceDetailRepository));
            _supplierInvoiceRepository = supplierInvoiceRepository ?? throw new ArgumentNullException(nameof(supplierInvoiceRepository));
            _supplierReturnInvoiceRepository = supplierReturnInvoiceRepository ?? throw new ArgumentNullException(nameof(supplierReturnInvoiceRepository));
            _supplierReturnInvoiceDetailRepository = supplierReturnInvoiceDetailRepository ?? throw new ArgumentNullException(nameof(supplierReturnInvoiceDetailRepository));
            _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
            _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(stockRepository));
            _purchaseEntryService = purchaseEntryService ?? throw new ArgumentNullException(nameof(purchaseEntryService));
        }

        public ISupplierInvoiceDetailRepository SupplierInvoiceDetailRepository => _supplierInvoiceDetailRepository;
        public ISupplierInvoiceRepository SupplierInvoiceRepository => _supplierInvoiceRepository;
        public ISupplierReturnInvoiceRepository SupplierReturnInvoiceRepository => _supplierReturnInvoiceRepository;
        public ISupplierReturnInvoiceDetailRepository SupplierReturnInvoiceDetailRepository => _supplierReturnInvoiceDetailRepository;
        public ISupplierRepository SupplierRepository => _supplierRepository;
        public IStockRepository StockRepository => _stockRepository;
        public IPurchaseEntryService PurchaseEntryService => _purchaseEntryService;
    }
}
