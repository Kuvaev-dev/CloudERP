using Domain.RepositoryAccess;
using Domain.Services;
using System;

namespace Domain.Facades
{
    public class SalePaymentFacade
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ICustomerReturnInvoiceRepository _customerReturnInvoiceRepository;
        private readonly ICustomerInvoiceDetailRepository _customerInvoiceDetailRepository;
        private readonly ISalePaymentService _salePaymentService;
        private readonly ISaleService _saleService;

        public SalePaymentFacade(
            ISaleRepository saleRepository,
            ICustomerReturnInvoiceRepository customerReturnInvoiceRepository,
            ICustomerInvoiceDetailRepository customerInvoiceDetailRepository,
            ISalePaymentService salePaymentService,
            ISaleService saleService)
        {
            _saleRepository = saleRepository ?? throw new ArgumentNullException(nameof(ISaleRepository));
            _customerReturnInvoiceRepository = customerReturnInvoiceRepository ?? throw new ArgumentNullException(nameof(ICustomerReturnInvoiceRepository));
            _customerInvoiceDetailRepository = customerInvoiceDetailRepository ?? throw new ArgumentNullException(nameof(ICustomerInvoiceDetailRepository));
            _salePaymentService = salePaymentService ?? throw new ArgumentNullException(nameof(ISalePaymentService));
            _saleService = saleService ?? throw new ArgumentNullException(nameof(ISaleService));
        }

        public ISaleRepository SaleRepository => _saleRepository;
        public ICustomerReturnInvoiceRepository CustomerReturnInvoiceRepository => _customerReturnInvoiceRepository;
        public ICustomerInvoiceDetailRepository CustomerInvoiceDetailRepository => _customerInvoiceDetailRepository;
        public ISalePaymentService SalePaymentService => _salePaymentService;
        public ISaleService SaleService => _saleService;
    }
}
