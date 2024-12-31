using Domain.RepositoryAccess;
using Domain.Services;

namespace CloudERP.Facades
{
    public class PurchaseCartFacade
    {
        private readonly IPurchaseEntryService _purchaseEntry;
        private readonly IStockRepository _stockRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IPurchaseCartDetailRepository _purchaseCartDetailRepository;
        private readonly ISupplierInvoiceRepository _supplierInvoiceRepository;
        private readonly ISupplierInvoiceDetailRepository _supplierInvoiceDetailRepository;

        public PurchaseCartFacade(
            IPurchaseEntryService purchaseEntry,
            IStockRepository stockRepository,
            ISupplierRepository supplierRepository,
            IPurchaseCartDetailRepository purchaseCartDetailRepository,
            ISupplierInvoiceRepository supplierInvoiceRepository,
            ISupplierInvoiceDetailRepository supplierInvoiceDetailRepository)
        {
            _purchaseEntry = purchaseEntry;
            _stockRepository = stockRepository;
            _supplierRepository = supplierRepository;
            _purchaseCartDetailRepository = purchaseCartDetailRepository;
            _supplierInvoiceRepository = supplierInvoiceRepository;
            _supplierInvoiceDetailRepository = supplierInvoiceDetailRepository;
        }

        public IPurchaseEntryService PurchaseEntry => _purchaseEntry;
        public IStockRepository StockRepository => _stockRepository;
        public ISupplierRepository SupplierRepository => _supplierRepository;
        public IPurchaseCartDetailRepository PurchaseCartDetailRepository => _purchaseCartDetailRepository;
        public ISupplierInvoiceRepository SupplierInvoiceRepository => _supplierInvoiceRepository;
        public ISupplierInvoiceDetailRepository SupplierInvoiceDetailRepository => _supplierInvoiceDetailRepository;
    }
}