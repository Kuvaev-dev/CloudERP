using Domain.RepositoryAccess;
using Domain.Services.Sale;
using System;

namespace Domain.Facades
{
    public class SaleCartFacade
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerInvoiceRepository _customerInvoiceRepository;
        private readonly ICustomerInvoiceDetailRepository _customerInvoiceDetailRepository;
        private readonly ISaleCartDetailRepository _saleCartDetailRepository;
        private readonly ISaleEntryService _saleEntryService;

        public SaleCartFacade(
            ICustomerRepository customerRepository,
            ICustomerInvoiceRepository customerInvoiceRepository,
            ICustomerInvoiceDetailRepository customerInvoiceDetailRepository,
            ISaleCartDetailRepository saleCartDetailRepository,
            ISaleEntryService saleEntryService)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(ICustomerRepository));
            _customerInvoiceRepository = customerInvoiceRepository ?? throw new ArgumentNullException(nameof(ICustomerInvoiceRepository));
            _customerInvoiceDetailRepository = customerInvoiceDetailRepository ?? throw new ArgumentNullException(nameof(ICustomerInvoiceDetailRepository));
            _saleCartDetailRepository = saleCartDetailRepository ?? throw new ArgumentNullException(nameof(ISaleCartDetailRepository));
            _saleEntryService = saleEntryService ?? throw new ArgumentNullException(nameof(ISaleEntryService));
        }

        public ICustomerRepository CustomerRepository => _customerRepository;
        public ICustomerInvoiceRepository CustomerInvoiceRepository => _customerInvoiceRepository;
        public ICustomerInvoiceDetailRepository CustomerInvoiceDetailRepository => _customerInvoiceDetailRepository;
        public ISaleCartDetailRepository SaleCartDetailRepository => _saleCartDetailRepository;
        public ISaleEntryService SaleEntryService => _saleEntryService;
    }
}
