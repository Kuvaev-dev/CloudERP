using Domain.RepositoryAccess;
using Domain.Services;

namespace Domain.Facades
{
    public class PurchaseCartFacade
    {
        private readonly ISupplierInvoiceRepository _supplierInvoiceRepository;
        private readonly ISupplierInvoiceDetailRepository _supplierInvoiceDetailRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IPurchaseCartDetailRepository _purchaseCartDetailRepository;
        private readonly IPurchaseEntryService _purchaseEntryService;

        public PurchaseCartFacade(
            ISupplierInvoiceRepository supplierInvoiceRepository,
            ISupplierInvoiceDetailRepository supplierInvoiceDetailRepository,
            ISupplierRepository supplierRepository,
            IStockRepository stockRepository,
            IPurchaseCartDetailRepository purchaseCartDetailRepository,
            IPurchaseEntryService purchaseEntryService)
        {
            _supplierInvoiceRepository = supplierInvoiceRepository;
            _supplierInvoiceDetailRepository = supplierInvoiceDetailRepository;
            _supplierRepository = supplierRepository;
            _stockRepository = stockRepository;
            _purchaseCartDetailRepository = purchaseCartDetailRepository;
            _purchaseEntryService = purchaseEntryService;
        }

        public ISupplierInvoiceRepository SupplierInvoiceRepository => _supplierInvoiceRepository;
        public ISupplierInvoiceDetailRepository SupplierInvoiceDetailRepository => _supplierInvoiceDetailRepository;
        public ISupplierRepository SupplierRepository => _supplierRepository;
        public IStockRepository StockRepository => _stockRepository;
        public IPurchaseCartDetailRepository PurchaseCartDetailRepository => _purchaseCartDetailRepository;
        public IPurchaseEntryService PurchaseEntryService => _purchaseEntryService;
    }
}
