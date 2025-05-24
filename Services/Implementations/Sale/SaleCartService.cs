using Domain.Facades;
using Domain.Models;
using Domain.ServiceAccess;

namespace Services.Implementations
{
    public class SaleCartService : ISaleCartService
    {
        private readonly SaleCartFacade _saleCartFacade;

        public SaleCartService(SaleCartFacade saleCartFacade)
        {
            _saleCartFacade = saleCartFacade ?? throw new ArgumentNullException(nameof(saleCartFacade));
        }

        public async Task<Result<int>> ConfirmSaleAsync(SaleConfirm saleConfirmDto)
        {
            try
            {
                var customer = await _saleCartFacade.CustomerRepository.GetByIdAsync(saleConfirmDto.CustomerID);
                var saleDetails = await _saleCartFacade.SaleCartDetailRepository.GetAllAsync(saleConfirmDto.BranchID, saleConfirmDto.CompanyID);

                double totalAmount = saleDetails.Sum(item => item.SaleQuantity * item.SaleUnitPrice);

                string invoiceNo = "INV" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var invoiceHeader = new CustomerInvoice
                {
                    BranchID = saleConfirmDto.BranchID,
                    Title = $"{Localization.Services.Localization.SaleInvoice}: {customer.Customername}",
                    CompanyID = saleConfirmDto.CompanyID,
                    Description = saleConfirmDto.Description,
                    InvoiceDate = DateTime.Now,
                    InvoiceNo = invoiceNo,
                    CustomerID = saleConfirmDto.CustomerID,
                    UserID = saleConfirmDto.UserID,
                    TotalAmount = totalAmount
                };

                await _saleCartFacade.CustomerInvoiceRepository.AddAsync(invoiceHeader);
                await _saleCartFacade.CustomerInvoiceDetailRepository.AddSaleDetailsAsync(saleDetails, invoiceHeader.CustomerInvoiceID);

                string message = await _saleCartFacade.SaleEntryService.ConfirmSale(
                    saleConfirmDto,
                    invoiceNo,
                    invoiceHeader.CustomerInvoiceID.ToString(),
                    (float)totalAmount,
                    customer.CustomerID.ToString(),
                    customer.Customername);

                if (message.Contains(Localization.Services.Localization.PurchaseSuccess))
                {
                    await _saleCartFacade.SaleEntryService.CompleteSale(saleDetails);
                    return Result<int>.Success(invoiceHeader.CustomerInvoiceID);
                }

                return Result<int>.Failure(message);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure("Unexpected Issue Is Occured: " + ex.Message);
            }
        }
    }
}
