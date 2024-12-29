using Domain.EntryAccess;
using Domain.RepositoryAccess;

namespace CloudERP.Facades
{
    public class SalePaymentFacade
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerInvoiceRepository _customerInvoiceRepository;
        private readonly ICustomerInvoiceDetailRepository _customerInvoiceDetailRepository;
        private readonly ICustomerReturnInvoiceRepository _customerReturnInvoiceRepository;
        private readonly ICustomerPaymentRepository _customerPaymentRepository;
        private readonly ISaleEntry _saleEntry;

        public SalePaymentFacade(
            ISaleRepository saleRepository,
            ICustomerRepository customerRepository,
            ICustomerInvoiceRepository customerInvoiceRepository,
            ICustomerInvoiceDetailRepository customerInvoiceDetailRepository,
            ICustomerReturnInvoiceRepository customerReturnInvoiceRepository,
            ICustomerPaymentRepository customerPaymentRepository,
            ISaleEntry saleEntry)
        {
            _saleRepository = saleRepository;
            _customerRepository = customerRepository;
            _customerInvoiceRepository = customerInvoiceRepository;
            _customerInvoiceDetailRepository = customerInvoiceDetailRepository;
            _customerReturnInvoiceRepository = customerReturnInvoiceRepository;
            _customerPaymentRepository = customerPaymentRepository;
            _saleEntry = saleEntry;
        }

        public ISaleRepository SaleRepository => _saleRepository;
        public ICustomerRepository CustomerRepository => _customerRepository;
        public ICustomerInvoiceRepository CustomerInvoiceRepository => _customerInvoiceRepository;
        public ICustomerInvoiceDetailRepository CustomerInvoiceDetailRepository => _customerInvoiceDetailRepository;
        public ICustomerReturnInvoiceRepository CustomerReturnInvoiceRepository => _customerReturnInvoiceRepository;
        public ICustomerPaymentRepository CustomerPaymentRepository => _customerPaymentRepository;
        public ISaleEntry SaleEntry => _saleEntry;
    }
}