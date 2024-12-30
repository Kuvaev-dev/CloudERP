using Domain.RepositoryAccess;
using Domain.Services;

namespace CloudERP.Facades
{
    public class SalePaymentReturnFacade
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerReturnPaymentRepository _customerReturnPaymentRepository;
        private readonly ICustomerReturnInvoiceRepository _customerReturnInvoiceRepository;
        private readonly ISaleEntryService _saleEntry;

        public SalePaymentReturnFacade(
            ISaleRepository saleRepository, 
            ICustomerRepository customerRepository, 
            ICustomerReturnPaymentRepository customerReturnPaymentRepository, 
            ICustomerReturnInvoiceRepository customerReturnInvoiceRepository, 
            ISaleEntryService saleEntry)
        {
            _saleRepository = saleRepository;
            _customerRepository = customerRepository;
            _customerReturnPaymentRepository = customerReturnPaymentRepository;
            _customerReturnInvoiceRepository = customerReturnInvoiceRepository;
            _saleEntry = saleEntry;
        }

        public ISaleRepository SaleRepository => _saleRepository;
        public ICustomerRepository CustomerRepository => _customerRepository;
        public ICustomerReturnPaymentRepository CustomerReturnPaymentRepository => _customerReturnPaymentRepository;
        public ICustomerReturnInvoiceRepository CustomerReturnInvoiceRepository => _customerReturnInvoiceRepository;
        public ISaleEntryService SaleEntry => _saleEntry;
    }
}