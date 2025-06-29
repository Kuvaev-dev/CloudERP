﻿using Domain.RepositoryAccess;
using Domain.ServiceAccess;

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
            _supplierInvoiceRepository = supplierInvoiceRepository ?? throw new ArgumentNullException(nameof(supplierInvoiceRepository));
            _supplierInvoiceDetailRepository = supplierInvoiceDetailRepository ?? throw new ArgumentNullException(nameof(supplierInvoiceDetailRepository));
            _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
            _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(stockRepository));
            _purchaseCartDetailRepository = purchaseCartDetailRepository ?? throw new ArgumentNullException(nameof(purchaseCartDetailRepository));
            _purchaseEntryService = purchaseEntryService ?? throw new ArgumentNullException(nameof(purchaseEntryService));
        }

        public ISupplierInvoiceRepository SupplierInvoiceRepository => _supplierInvoiceRepository;
        public ISupplierInvoiceDetailRepository SupplierInvoiceDetailRepository => _supplierInvoiceDetailRepository;
        public ISupplierRepository SupplierRepository => _supplierRepository;
        public IStockRepository StockRepository => _stockRepository;
        public IPurchaseCartDetailRepository PurchaseCartDetailRepository => _purchaseCartDetailRepository;
        public IPurchaseEntryService PurchaseEntryService => _purchaseEntryService;
    }
}
