using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Sale
{
    public class SaleReturnService : ISaleReturnService
    {
        private readonly ICustomerInvoiceDetailRepository _customerInvoiceDetailRepository;
        private readonly ICustomerInvoiceRepository _customerInvoiceRepository;
        private readonly ICustomerReturnInvoiceRepository _customerReturnInvoiceRepository;
        private readonly ICustomerReturnInvoiceDetailRepository _customerReturnInvoiceDetailRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ISaleEntryService _saleEntryService;

        public SaleReturnService(
            ICustomerInvoiceDetailRepository customerInvoiceDetailRepository,
            ICustomerInvoiceRepository customerInvoiceRepository,
            ICustomerReturnInvoiceRepository customerReturnInvoiceRepository,
            ICustomerReturnInvoiceDetailRepository customerReturnInvoiceDetailRepository,
            ICustomerRepository customerRepository,
            IStockRepository stockRepository,
            ISaleEntryService saleEntryService)
        {
            _customerInvoiceDetailRepository = customerInvoiceDetailRepository ?? throw new ArgumentNullException(nameof(ICustomerInvoiceDetailRepository));
            _customerInvoiceRepository = customerInvoiceRepository ?? throw new ArgumentNullException(nameof(ICustomerInvoiceRepository));
            _customerReturnInvoiceRepository = customerReturnInvoiceRepository ?? throw new ArgumentNullException(nameof(ICustomerReturnInvoiceRepository));
            _customerReturnInvoiceDetailRepository = customerReturnInvoiceDetailRepository ?? throw new ArgumentNullException(nameof(ICustomerReturnInvoiceDetailRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(ICustomerRepository));
            _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(IStockRepository));
            _saleEntryService = saleEntryService ?? throw new ArgumentNullException(nameof(ISaleEntryService));
        }

        public async Task<ReturnConfirmResult> ProcessReturnConfirmAsync(SaleReturnConfirm returnConfirmDto, int branchId, int companyId, int userId)
        {
            double totalAmount = 0;
            var saleDetails = await _customerInvoiceDetailRepository.GetListByIdAsync(returnConfirmDto.CustomerInvoiceID);
            var list = saleDetails.ToList();

            for (int i = 0; i < saleDetails.Count(); i++)
            {
                foreach (var productID in returnConfirmDto.ProductIDs)
                {
                    if (productID == list[i].ProductID)
                    {
                        totalAmount += (returnConfirmDto.ReturnQty[i] * list[i].SaleUnitPrice);
                    }
                }
            }

            var customerInvoice = await _customerInvoiceRepository.GetByIdAsync(returnConfirmDto.CustomerInvoiceID);
            int customerID = customerInvoice.CustomerID;

            if (totalAmount == 0)
            {
                return new ReturnConfirmResult { IsSuccess = false, Message = "One Product Return Qty Error", InvoiceNo = string.Empty };
            }

            string invoiceNo = "RIN" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
            var returnInvoiceHeader = new CustomerReturnInvoice()
            {
                BranchID = branchId,
                CompanyID = companyId,
                Description = "Sale Return",
                InvoiceDate = DateTime.Now,
                InvoiceNo = invoiceNo,
                CustomerID = customerID,
                UserID = userId,
                TotalAmount = totalAmount,
                CustomerInvoiceID = returnConfirmDto.CustomerInvoiceID
            };

            await _customerReturnInvoiceRepository.AddAsync(returnInvoiceHeader);

            var customer = await _customerRepository.GetByIdAsync(customerID);
            string message = await _saleEntryService.ReturnSale(
                companyId,
                branchId,
                userId,
                invoiceNo,
                returnInvoiceHeader.CustomerInvoiceID.ToString(),
                returnInvoiceHeader.CustomerReturnInvoiceID,
                (float)totalAmount,
                customerID.ToString(),
                customer.Customername,
                returnConfirmDto.IsPayment);

            if (message.Contains("Success"))
            {
                for (int i = 0; i < saleDetails.Count(); i++)
                {
                    foreach (var productID in returnConfirmDto.ProductIDs)
                    {
                        if (productID == list[i].ProductID && returnConfirmDto.ReturnQty[i] > 0)
                        {
                            var returnProductDetails = new CustomerReturnInvoiceDetail()
                            {
                                CustomerInvoiceID = returnConfirmDto.CustomerInvoiceID,
                                SaleReturnQuantity = returnConfirmDto.ReturnQty[i],
                                ProductID = productID,
                                SaleReturnUnitPrice = list[i].SaleUnitPrice,
                                CustomerReturnInvoiceID = returnInvoiceHeader.CustomerReturnInvoiceID,
                                CustomerInvoiceDetailID = list[i].CustomerInvoiceDetailID
                            };

                            await _customerReturnInvoiceDetailRepository.AddAsync(returnProductDetails);

                            var stock = await _stockRepository.GetByIdAsync(productID);
                            if (stock != null)
                            {
                                stock.Quantity += returnConfirmDto.ReturnQty[i];
                                await _stockRepository.UpdateAsync(stock);
                            }
                        }
                    }
                }

                return new ReturnConfirmResult { IsSuccess = true, Message = "Return Successfully", InvoiceNo = invoiceNo };
            }

            return new ReturnConfirmResult { IsSuccess = false, Message = "Unexpected Issue", InvoiceNo = invoiceNo };
        }
    }
}
