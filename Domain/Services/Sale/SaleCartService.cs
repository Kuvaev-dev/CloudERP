using Domain.Facades;
using Domain.Models;
using Domain.Models.FinancialModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services.Sale
{
    public interface ISaleCartService
    {
        Task<Result> ConfirmSaleAsync(SaleConfirm saleConfirmDto, int branchId, int companyId, int userId);
    }

    public class SaleCartService : ISaleCartService
    {
        private readonly SaleCartFacade _saleCartFacade;

        public SaleCartService(SaleCartFacade saleCartFacade)
        {
            _saleCartFacade = saleCartFacade ?? throw new ArgumentNullException(nameof(SaleCartFacade));
        }

        public async Task<Result> ConfirmSaleAsync(SaleConfirm saleConfirmDto, int branchId, int companyId, int userId)
        {
            try
            {
                var customer = await _saleCartFacade.CustomerRepository.GetByIdAsync(saleConfirmDto.CustomerID);
                var saleDetails = await _saleCartFacade.SaleCartDetailRepository.GetAllAsync(branchId, companyId);

                double totalAmount = saleDetails.Sum(item => item.SaleQuantity * item.SaleUnitPrice);

                if (totalAmount == 0)
                {
                    return new Result(false, "Корзина продаж пуста.");
                }

                string invoiceNo = "INV" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var invoiceHeader = new CustomerInvoice
                {
                    BranchID = branchId,
                    Title = "Sale Invoice " + customer.Customername,
                    CompanyID = companyId,
                    Description = saleConfirmDto.Description,
                    InvoiceDate = DateTime.Now,
                    InvoiceNo = invoiceNo,
                    CustomerID = saleConfirmDto.CustomerID,
                    UserID = userId,
                    TotalAmount = totalAmount
                };

                await _saleCartFacade.CustomerInvoiceRepository.AddAsync(invoiceHeader);
                await _saleCartFacade.CustomerInvoiceDetailRepository.AddSaleDetailsAsync(saleDetails, invoiceHeader.CustomerInvoiceID);

                string message = await _saleCartFacade.SaleEntryService.ConfirmSale(
                    companyId,
                    branchId,
                    userId,
                    invoiceNo,
                    invoiceHeader.CustomerInvoiceID.ToString(),
                    (float)totalAmount,
                    saleConfirmDto.CustomerID.ToString(),
                    customer.Customername,
                    saleConfirmDto.IsPayment);

                if (message.Contains("Success"))
                {
                    await _saleCartFacade.SaleEntryService.CompleteSale(saleDetails);
                    return new Result(true, invoiceHeader.CustomerInvoiceID.ToString());
                }

                return new Result(false, message);
            }
            catch (Exception ex)
            {
                return new Result(false, "Произошла непредвиденная ошибка: " + ex.Message);
            }
        }
    }
}
