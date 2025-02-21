using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Sale
{
    public class SalePaymentReturnService : ISalePaymentReturnService
    {
        private readonly ICustomerReturnPaymentRepository _customerReturnPaymentRepository;
        private readonly ICustomerReturnInvoiceRepository _customerReturnInvoiceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISaleEntryService _saleEntryService;

        public SalePaymentReturnService(
            ICustomerReturnPaymentRepository customerReturnPaymentRepository,
            ICustomerReturnInvoiceRepository customerReturnInvoiceRepository,
            ICustomerRepository customerRepository,
            ISaleEntryService saleEntryService)
        {
            _customerReturnPaymentRepository = customerReturnPaymentRepository ?? throw new ArgumentNullException(nameof(ICustomerReturnPaymentRepository));
            _customerReturnInvoiceRepository = customerReturnInvoiceRepository ?? throw new ArgumentNullException(nameof(ICustomerReturnInvoiceRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(ICustomerRepository));
            _saleEntryService = saleEntryService ?? throw new ArgumentNullException(nameof(ISaleEntryService));
        }

        public async Task<(bool IsSuccess, string Message, IEnumerable<CustomerReturnPayment> Items, double RemainingAmount)> ProcessReturnAmountAsync(SalePaymentReturn paymentReturnDto, int branchId, int companyId, int userId)
        {
            if (paymentReturnDto.PaymentAmount > paymentReturnDto.PreviousRemainingAmount)
            {
                var list = await _customerReturnPaymentRepository.GetListByReturnInvoiceIdAsync(paymentReturnDto.InvoiceId);
                double remainingAmount = list.Sum(item => item.RemainingBalance);

                if (remainingAmount == 0)
                {
                    remainingAmount = await _customerReturnInvoiceRepository.GetTotalAmountByIdAsync(paymentReturnDto.InvoiceId);
                }

                return (false, "Purchase Payment Remaining Amount Error", list, remainingAmount);
            }

            string payInvoiceNo = "RIP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
            var customerFromReturnInvoice = await _customerReturnInvoiceRepository.GetByIdAsync(paymentReturnDto.InvoiceId);
            var customer = await _customerRepository.GetByIdAsync(customerFromReturnInvoice.CustomerID);

            string message = await _saleEntryService.ReturnSalePayment(
                companyId,
                branchId,
                userId,
                payInvoiceNo,
                customerFromReturnInvoice.CustomerInvoiceID.ToString(),
                customerFromReturnInvoice.CustomerReturnInvoiceID,
                (float)customerFromReturnInvoice.TotalAmount,
                paymentReturnDto.PaymentAmount,
                customer.CustomerID.ToString(),
                customer.Customername,
                paymentReturnDto.PreviousRemainingAmount - paymentReturnDto.PaymentAmount);

            return (true, message, null, 0);
        }
    }
}
