using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace Services.Implementations
{
    public class SalePaymentService : ISalePaymentService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ICustomerInvoiceRepository _customerInvoiceRepository;
        private readonly ICustomerPaymentRepository _customerPaymentRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISaleEntryService _saleEntryService;

        public SalePaymentService(
            ISaleRepository saleRepository,
            ICustomerInvoiceRepository customerInvoiceRepository,
            ICustomerPaymentRepository customerPaymentRepository,
            ICustomerRepository customerRepository,
            ISaleEntryService saleEntryService)
        {
            _saleRepository = saleRepository ?? throw new ArgumentNullException(nameof(ISaleRepository));
            _customerInvoiceRepository = customerInvoiceRepository ?? throw new ArgumentNullException(nameof(ICustomerInvoiceRepository));
            _customerPaymentRepository = customerPaymentRepository ?? throw new ArgumentNullException(nameof(ICustomerPaymentRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(ICustomerRepository));
            _saleEntryService = saleEntryService ?? throw new ArgumentNullException(nameof(ISaleEntryService));
        }

        public async Task<string> ProcessPaymentAsync(int companyId, int branchId, int userId, SaleAmount paymentDto)
        {
            if (paymentDto.PaidAmount > paymentDto.PreviousRemainingAmount)
            {
                return Localization.Services.Localization.SalePaymentRemainingAmountError;
            }

            string payInvoiceNo = "INP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
            var customerInvoice = await _customerInvoiceRepository.GetByIdAsync(paymentDto.InvoiceId);
            var customer = await _customerRepository.GetByIdAsync(customerInvoice.CustomerID);

            string message = await _saleEntryService.SalePayment(
                companyId,
                branchId,
                userId,
                payInvoiceNo,
                Convert.ToString(paymentDto.InvoiceId),
                (float)customerInvoice.TotalAmount,
                paymentDto.PaidAmount,
                Convert.ToString(customer.CustomerID),
                customer.Customername,
                paymentDto.PreviousRemainingAmount - paymentDto.PaidAmount);

            return message;
        }
    }
}
