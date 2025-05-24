using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace Services.Facades
{
    public class SalePaymentFacade
    {
        private readonly ISalePaymentService _salePaymentService;
        private readonly ISaleRepository _saleRepository;
        private readonly ISaleService _saleService;
        private readonly ICustomerReturnInvoiceRepository _customerReturnInvoiceRepository;
        private readonly ICustomerInvoiceDetailRepository _customerInvoiceDetailRepository;
        private readonly ICustomerInvoiceRepository _customerInvoiceRepository;
        private readonly ICustomerPaymentRepository _customerPaymentRepository;

        public SalePaymentFacade(
            ISalePaymentService salePaymentService,
            ISaleRepository saleRepository,
            ISaleService saleService,
            ICustomerReturnInvoiceRepository customerReturnInvoiceRepository,
            ICustomerInvoiceDetailRepository customerInvoiceDetailRepository,
            ICustomerInvoiceRepository customerInvoiceRepository,
            ICustomerPaymentRepository customerPaymentRepository)
        {
            _salePaymentService = salePaymentService ?? throw new ArgumentNullException(nameof(salePaymentService));
            _saleRepository = saleRepository ?? throw new ArgumentNullException(nameof(saleRepository));
            _saleService = saleService ?? throw new ArgumentNullException(nameof(saleService));
            _customerReturnInvoiceRepository = customerReturnInvoiceRepository ?? throw new ArgumentNullException(nameof(customerReturnInvoiceRepository));
            _customerInvoiceDetailRepository = customerInvoiceDetailRepository ?? throw new ArgumentNullException(nameof(customerInvoiceDetailRepository));
            _customerInvoiceRepository = customerInvoiceRepository;
            _customerPaymentRepository = customerPaymentRepository;
        }

        public ISalePaymentService SalePaymentService => _salePaymentService;
        public ISaleRepository SaleRepository => _saleRepository;
        public ISaleService SaleService => _saleService;
        public ICustomerReturnInvoiceRepository CustomerReturnInvoiceRepository => _customerReturnInvoiceRepository;
        public ICustomerInvoiceDetailRepository CustomerInvoiceDetailRepository => _customerInvoiceDetailRepository;
        public ICustomerInvoiceRepository CustomerInvoiceRepository => _customerInvoiceRepository;
        public ICustomerPaymentRepository CustomerPaymentRepository => _customerPaymentRepository;
    }
}
