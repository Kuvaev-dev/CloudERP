using Domain.RepositoryAccess;
using Domain.Services;

namespace CloudERP.Facades
{
    public class SaleCartFacade
    {
        private readonly ISaleEntryService _saleEntry;
        private readonly ISaleCartDetailRepository _saleCartDetailRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerInvoiceRepository _customerInvoiceRepository;
        private readonly ICustomerInvoiceDetailRepository _customerInvoiceDetailRepository;

        public SaleCartFacade(
            ISaleEntryService saleEntry,
            ISaleCartDetailRepository saleCartDetailRepository,
            IStockRepository stockRepository,
            ICustomerRepository customerRepository,
            ICustomerInvoiceRepository customerInvoiceRepository,
            ICustomerInvoiceDetailRepository customerInvoiceDetailRepository)
        {
            _saleEntry = saleEntry;
            _saleCartDetailRepository = saleCartDetailRepository;
            _stockRepository = stockRepository;
            _customerRepository = customerRepository;
            _customerInvoiceRepository = customerInvoiceRepository;
            _customerInvoiceDetailRepository = customerInvoiceDetailRepository;
        }

        public ISaleEntryService SaleEntry => _saleEntry;
        public ISaleCartDetailRepository SaleCartDetailRepository => _saleCartDetailRepository;
        public IStockRepository StockRepository => _stockRepository;
        public ICustomerRepository CustomerRepository => _customerRepository;
        public ICustomerInvoiceRepository CustomerInvoiceRepository => _customerInvoiceRepository;
        public ICustomerInvoiceDetailRepository CustomerInvoiceDetailRepository => _customerInvoiceDetailRepository;
    }
}