using Domain.RepositoryAccess;
using Domain.Services;

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
            _supplierInvoiceDetailRepository = supplierInvoiceDetailRepository;
            _supplierInvoiceRepository = supplierInvoiceRepository;
            _supplierReturnInvoiceRepository = supplierReturnInvoiceRepository;
            _supplierReturnInvoiceDetailRepository = supplierReturnInvoiceDetailRepository;
            _supplierRepository = supplierRepository;
            _stockRepository = stockRepository;
            _purchaseEntryService = purchaseEntryService;
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
