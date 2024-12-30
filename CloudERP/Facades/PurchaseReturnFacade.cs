using Domain.RepositoryAccess;
using Domain.Services;

namespace CloudERP.Facades
{
    public class PurchaseReturnFacade
    {
        private readonly IPurchaseEntryService _purchaseEntry;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ISupplierInvoiceRepository _supplierInvoiceRepository;
        private readonly ISupplierReturnInvoiceRepository _supplierReturnInvoiceRepository;
        private readonly ISupplierInvoiceDetailRepository _supplierInvoiceDetailRepository;
        private readonly ISupplierReturnInvoiceDetailRepository _supplierReturnInvoiceDetailRepository;
        private readonly IStockRepository _stockRepository;

        public PurchaseReturnFacade(
            IPurchaseEntryService purchaseEntry,
            ISupplierRepository supplierRepository,
            ISupplierInvoiceRepository supplierInvoiceRepository,
            ISupplierReturnInvoiceRepository supplierReturnInvoiceRepository,
            ISupplierInvoiceDetailRepository supplierInvoiceDetailRepository,
            ISupplierReturnInvoiceDetailRepository supplierReturnInvoiceDetailRepository,
            IStockRepository stockRepository)
        {
            _purchaseEntry = purchaseEntry;
            _supplierRepository = supplierRepository;
            _supplierInvoiceRepository = supplierInvoiceRepository;
            _supplierReturnInvoiceRepository = supplierReturnInvoiceRepository;
            _supplierInvoiceDetailRepository = supplierInvoiceDetailRepository;
            _supplierReturnInvoiceDetailRepository = supplierReturnInvoiceDetailRepository;
            _stockRepository = stockRepository;
        }

        public IPurchaseEntryService PurchaseEntry => _purchaseEntry;
        public ISupplierRepository SupplierRepository => _supplierRepository;
        public ISupplierInvoiceRepository SupplierInvoiceRepository => _supplierInvoiceRepository;
        public ISupplierReturnInvoiceRepository SupplierReturnInvoiceRepository => _supplierReturnInvoiceRepository;
        public ISupplierInvoiceDetailRepository SupplierInvoiceDetailRepository => _supplierInvoiceDetailRepository;
        public ISupplierReturnInvoiceDetailRepository SupplierReturnInvoiceDetailRepository => _supplierReturnInvoiceDetailRepository;
        public IStockRepository StockRepository => _stockRepository;
    }
}