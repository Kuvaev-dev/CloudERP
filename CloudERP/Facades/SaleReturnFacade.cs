using Domain.RepositoryAccess;
using Domain.Services;

namespace CloudERP.Facades
{
    public class SaleReturnFacade
    {
        private readonly ISaleEntryService _saleEntry;
        private readonly IStockRepository _stockRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerInvoiceRepository _customerInvoiceRepository;
        private readonly ICustomerInvoiceDetailRepository _customerInvoiceDetailRepository;
        private readonly ICustomerReturnInvoiceRepository _customerReturnInvoiceRepository;
        private readonly ICustomerReturnInvoiceDetailRepository _customerReturnInvoiceDetailRepository;

        public SaleReturnFacade(
            ISaleEntryService saleEntry,
            IStockRepository stockRepository,
            ICustomerRepository customerRepository,
            ICustomerInvoiceRepository customerInvoiceRepository,
            ICustomerInvoiceDetailRepository customerInvoiceDetailRepository,
            ICustomerReturnInvoiceRepository customerReturnInvoiceRepository,
            ICustomerReturnInvoiceDetailRepository customerReturnInvoiceDetailRepository)
        {
            _saleEntry = saleEntry;
            _stockRepository = stockRepository;
            _customerRepository = customerRepository;
            _customerInvoiceRepository = customerInvoiceRepository;
            _customerInvoiceDetailRepository = customerInvoiceDetailRepository;
            _customerReturnInvoiceRepository = customerReturnInvoiceRepository;
            _customerReturnInvoiceDetailRepository = customerReturnInvoiceDetailRepository;
        }

        public ISaleEntryService SaleEntry => _saleEntry;
        public IStockRepository StockRepository => _stockRepository;
        public ICustomerRepository CustomerRepository => _customerRepository;
        public ICustomerInvoiceRepository CustomerInvoiceRepository => _customerInvoiceRepository;
        public ICustomerInvoiceDetailRepository CustomerInvoiceDetailRepository => _customerInvoiceDetailRepository;
        public ICustomerReturnInvoiceRepository CustomerReturnInvoiceRepository => _customerReturnInvoiceRepository;
        public ICustomerReturnInvoiceDetailRepository CustomerReturnInvoiceDetailRepository => _customerReturnInvoiceDetailRepository;
    }
}