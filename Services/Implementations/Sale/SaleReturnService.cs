using Domain.Models;
using Domain.ServiceAccess;
using Services.Facades;

namespace Services.Implementations
{
    public class SaleReturnService : ISaleReturnService
    {
        private readonly SaleReturnFacade _saleReturnFacade;

        public SaleReturnService(SaleReturnFacade saleReturnFacade)
        {
            _saleReturnFacade = saleReturnFacade ?? throw new ArgumentNullException(nameof(saleReturnFacade));
        }

        public async Task<SaleReturnConfirmResult> ProcessReturnConfirmAsync(SaleReturnConfirm returnConfirmDto, int branchId, int companyId, int userId)
        {
            double totalAmount = 0;
            var saleDetails = await _saleReturnFacade.CustomerInvoiceDetailRepository.GetListByIdAsync(returnConfirmDto.CustomerInvoiceID);
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

            var customerInvoice = await _saleReturnFacade.CustomerInvoiceRepository.GetByIdAsync(returnConfirmDto.CustomerInvoiceID);
            int customerID = customerInvoice.CustomerID;

            if (totalAmount == 0)
            {
                return new SaleReturnConfirmResult { IsSuccess = false, Message = Localization.Services.Localization.OneProductReturnQtyError, InvoiceNo = string.Empty };
            }

            string invoiceNo = "RIN" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
            var returnInvoiceHeader = new CustomerReturnInvoice()
            {
                BranchID = branchId,
                CompanyID = companyId,
                Description = Localization.Services.Localization.SaleReturn,
                InvoiceDate = DateTime.Now,
                InvoiceNo = invoiceNo,
                CustomerID = customerID,
                UserID = userId,
                TotalAmount = totalAmount,
                CustomerInvoiceID = returnConfirmDto.CustomerInvoiceID
            };

            await _saleReturnFacade.CustomerReturnInvoiceRepository.AddAsync(returnInvoiceHeader);

            var customer = await _saleReturnFacade.CustomerRepository.GetByIdAsync(customerID);
            string message = await _saleReturnFacade.SaleEntryService.ReturnSale(
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

                            await _saleReturnFacade.CustomerReturnInvoiceDetailRepository.AddAsync(returnProductDetails);

                            var stock = await _saleReturnFacade.StockRepository.GetByIdAsync(productID);
                            if (stock != null)
                            {
                                stock.Quantity += returnConfirmDto.ReturnQty[i];
                                await _saleReturnFacade.StockRepository.UpdateAsync(stock);
                            }
                        }
                    }
                }

                return new SaleReturnConfirmResult { IsSuccess = true, Message = Localization.Services.Localization.ReturnSuccessfully, InvoiceNo = invoiceNo };
            }

            return new SaleReturnConfirmResult { IsSuccess = false, Message = Localization.Services.Localization.UnexpectedIssue, InvoiceNo = invoiceNo };
        }
    }
}
