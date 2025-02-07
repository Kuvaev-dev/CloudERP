using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface ISalePaymentService
    {
        Task<List<SalePaymentModel>> GetSalePaymentHistoryAsync(int invoiceId);
        Task<double> GetTotalAmountByIdAsync(int invoiceId);
        Task<double> GetTotalPaidAmountByIdAsync(int invoiceId);
        Task<string> ProcessPaymentAsync(SalePayment paymentDto, int branchId, int companyId, int userId);
    }

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

        public async Task<List<SalePaymentModel>> GetSalePaymentHistoryAsync(int invoiceId)
        {
            return await _saleRepository.SalePaymentHistory(invoiceId);
        }

        public async Task<double> GetTotalAmountByIdAsync(int invoiceId)
        {
            return await _customerInvoiceRepository.GetTotalAmountByIdAsync(invoiceId);
        }

        public async Task<double> GetTotalPaidAmountByIdAsync(int invoiceId)
        {
            return await _customerPaymentRepository.GetTotalPaidAmountById(invoiceId);
        }

        public async Task<string> ProcessPaymentAsync(SalePayment paymentDto, int branchId, int companyId, int userId)
        {
            if (paymentDto.PaidAmount > paymentDto.PreviousRemainingAmount)
            {
                return "Sale Payment Remaining Amount Error";
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
