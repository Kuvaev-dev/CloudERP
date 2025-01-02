using Domain.Facades;
using Domain.Models;
using Domain.Models.FinancialModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services.Purchase
{
    public interface IPurchaseReturnService
    {
        Task<Result> ProcessReturnAsync(PurchaseReturnConfirm returnDto, int branchId, int companyId, int userId);
    }

    public class PurchaseReturnService : IPurchaseReturnService
    {
        private readonly PurchaseReturnFacade _purchaseReturnFacade;

        public PurchaseReturnService(PurchaseReturnFacade purchaseReturnFacade)
        {
            _purchaseReturnFacade = purchaseReturnFacade ?? throw new ArgumentNullException(nameof(PurchaseReturnFacade));
        }

        public async Task<Result> ProcessReturnAsync(PurchaseReturnConfirm returnDto, int branchId, int companyId, int userId)
        {
            try
            {
                double totalAmount = 0;
                var purchaseDetails = await _purchaseReturnFacade.SupplierInvoiceDetailRepository.GetListByIdAsync(returnDto.SupplierInvoiceID);
                if (purchaseDetails == null || !purchaseDetails.Any())
                {
                    return new Result(false, "Детали счета не найдены.");
                }

                foreach (var productReturn in returnDto.ProductReturns)
                {
                    var purchaseDetail = purchaseDetails.FirstOrDefault(pd => pd.ProductID == productReturn.ProductID);
                    if (purchaseDetail != null)
                    {
                        totalAmount += productReturn.ReturnQuantity * purchaseDetail.PurchaseUnitPrice;
                    }
                }

                var supplierInvoice = await _purchaseReturnFacade.SupplierInvoiceRepository.GetByIdAsync(returnDto.SupplierInvoiceID);
                int supplierID = supplierInvoice.SupplierID;

                if (totalAmount == 0)
                {
                    return new Result(false, "Количество возвращаемых товаров должно быть больше нуля.");
                }

                string invoiceNo = "RPU" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var returnInvoiceHeader = new SupplierReturnInvoice()
                {
                    BranchID = branchId,
                    CompanyID = companyId,
                    Description = "Возврат покупки",
                    InvoiceDate = DateTime.Now,
                    InvoiceNo = invoiceNo,
                    SupplierID = supplierID,
                    UserID = userId,
                    TotalAmount = totalAmount,
                    SupplierInvoiceID = returnDto.SupplierInvoiceID
                };
                await _purchaseReturnFacade.SupplierReturnInvoiceRepository.AddAsync(returnInvoiceHeader);

                var supplier = await _purchaseReturnFacade.SupplierRepository.GetByIdAsync(supplierID);
                string message = await _purchaseReturnFacade.PurchaseEntryService.ReturnPurchase(
                    companyId,
                    branchId,
                    userId,
                    invoiceNo,
                    returnInvoiceHeader.SupplierInvoiceID.ToString(),
                    returnInvoiceHeader.SupplierReturnInvoiceID,
                    (float)totalAmount,
                    supplierID.ToString(),
                    supplier.SupplierName,
                    returnDto.IsPayment);

                if (message.Contains("Success"))
                {
                    foreach (var productReturn in returnDto.ProductReturns)
                    {
                        var purchaseDetail = purchaseDetails.FirstOrDefault(pd => pd.ProductID == productReturn.ProductID);
                        if (purchaseDetail != null && productReturn.ReturnQuantity > 0)
                        {
                            var returnProductDetails = new SupplierReturnInvoiceDetail()
                            {
                                SupplierInvoiceID = returnDto.SupplierInvoiceID,
                                PurchaseReturnQuantity = productReturn.ReturnQuantity,
                                ProductID = productReturn.ProductID,
                                PurchaseReturnUnitPrice = purchaseDetail.PurchaseUnitPrice,
                                SupplierReturnInvoiceID = returnInvoiceHeader.SupplierReturnInvoiceID,
                                SupplierInvoiceDetailID = purchaseDetail.SupplierInvoiceDetailID
                            };
                            await _purchaseReturnFacade.SupplierReturnInvoiceDetailRepository.AddAsync(returnProductDetails);

                            var stock = await _purchaseReturnFacade.StockRepository.GetByIdAsync(productReturn.ProductID);
                            if (stock != null)
                            {
                                stock.Quantity -= productReturn.ReturnQuantity;
                                await _purchaseReturnFacade.StockRepository.UpdateAsync(stock);
                            }
                        }
                    }

                    return new Result(true, "Возврат успешно обработан.");
                }

                return new Result(false, "Произошла ошибка при обработке возврата.");
            }
            catch (Exception ex)
            {
                return new Result(false, "Произошла непредвиденная ошибка: " + ex.Message);
            }
        }
    }
}
