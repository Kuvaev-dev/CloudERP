using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace Services.Facades
{
    public class SaleReturnFacade
    {
        private readonly ICustomerInvoiceDetailRepository _customerInvoiceDetailRepository;
        private readonly ICustomerInvoiceRepository _customerInvoiceRepository;
        private readonly ICustomerReturnInvoiceRepository _customerReturnInvoiceRepository;
        private readonly ICustomerReturnInvoiceDetailRepository _customerReturnInvoiceDetailRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ISaleEntryService _saleEntryService;

        public SaleReturnFacade(
            ICustomerInvoiceDetailRepository customerInvoiceDetailRepository,
            ICustomerInvoiceRepository customerInvoiceRepository,
            ICustomerReturnInvoiceRepository customerReturnInvoiceRepository,
            ICustomerReturnInvoiceDetailRepository customerReturnInvoiceDetailRepository,
            ICustomerRepository customerRepository,
            IStockRepository stockRepository,
            ISaleEntryService saleEntryService)
        {
            _customerInvoiceDetailRepository = customerInvoiceDetailRepository ?? throw new ArgumentNullException(nameof(customerInvoiceDetailRepository));
            _customerInvoiceRepository = customerInvoiceRepository ?? throw new ArgumentNullException(nameof(customerInvoiceRepository));
            _customerReturnInvoiceRepository = customerReturnInvoiceRepository ?? throw new ArgumentNullException(nameof(customerReturnInvoiceRepository));
            _customerReturnInvoiceDetailRepository = customerReturnInvoiceDetailRepository ?? throw new ArgumentNullException(nameof(customerReturnInvoiceDetailRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(stockRepository));
            _saleEntryService = saleEntryService ?? throw new ArgumentNullException(nameof(saleEntryService));
        }

        public ICustomerInvoiceDetailRepository CustomerInvoiceDetailRepository => _customerInvoiceDetailRepository;
        public ICustomerInvoiceRepository CustomerInvoiceRepository => _customerInvoiceRepository;
        public ICustomerReturnInvoiceRepository CustomerReturnInvoiceRepository => _customerReturnInvoiceRepository;
        public ICustomerReturnInvoiceDetailRepository CustomerReturnInvoiceDetailRepository => _customerReturnInvoiceDetailRepository;
        public ICustomerRepository CustomerRepository => _customerRepository;
        public IStockRepository StockRepository => _stockRepository;
        public ISaleEntryService SaleEntryService => _saleEntryService;
    }
}
