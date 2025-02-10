using Domain.Facades;
using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.ServiceAccess;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utils.Helpers;

namespace Domain.Services
{
    public class SaleCartService : ISaleCartService
    {
        private readonly SaleCartFacade _saleCartFacade;

        public SaleCartService(SaleCartFacade saleCartFacade)
        {
            _saleCartFacade = saleCartFacade ?? throw new ArgumentNullException(nameof(saleCartFacade));
        }

        public async Task<Result<int>> ConfirmSaleAsync(SaleConfirm saleConfirmDto, int branchId, int companyId, int userId)
        {
            try
            {
                var customer = await _saleCartFacade.CustomerRepository.GetByIdAsync(saleConfirmDto.CustomerID);
                var saleDetails = await _saleCartFacade.SaleCartDetailRepository.GetAllAsync(branchId, companyId);

                double totalAmount = saleDetails.Sum(item => item.SaleQuantity * item.SaleUnitPrice);

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
