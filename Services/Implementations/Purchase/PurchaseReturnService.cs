using Domain.Facades;
using Domain.Models;
using Domain.ServiceAccess;

namespace Services.Implementations
{
    public class PurchaseReturnService : IPurchaseReturnService
    {
        private readonly PurchaseReturnFacade _purchaseReturnFacade;

        public PurchaseReturnService(PurchaseReturnFacade purchaseReturnFacade)
        {
            _purchaseReturnFacade = purchaseReturnFacade ?? throw new ArgumentNullException(nameof(purchaseReturnFacade));
        }

        public async Task<(bool IsSuccess, string Message, string InvoiceNo)> ProcessReturnAsync(PurchaseReturnConfirm returnConfirmDto, int branchId, int companyId, int userId)
        {
            double totalAmount = 0;
            var purchaseDetails = await _purchaseReturnFacade.SupplierInvoiceDetailRepository.GetListByIdAsync(returnConfirmDto.SupplierInvoiceID);
            var list = purchaseDetails.ToList();

            for (int i = 0; i < purchaseDetails.Count(); i++)
            {
                foreach (var productID in returnConfirmDto.ProductIDs)
                {
                    if (productID == list[i].ProductID)
                    {
                        totalAmount += returnConfirmDto.ReturnQty[i] * list[i].PurchaseUnitPrice;
                    }
                }
            }

            var supplierInvoice = await _purchaseReturnFacade.SupplierInvoiceRepository.GetByIdAsync(returnConfirmDto.SupplierInvoiceID);
            int supplierID = supplierInvoice.SupplierID;

            if (totalAmount == 0)
            {
                return (false, "One Product Return Qty Error", string.Empty);
            }

            string invoiceNo = "RPU" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
            var returnInvoiceHeader = new SupplierReturnInvoice()
            {
                BranchID = branchId,
                CompanyID = companyId,
                Description = "Purchase Return",
                InvoiceDate = DateTime.Now,
                InvoiceNo = invoiceNo,
                SupplierID = supplierID,
                UserID = userId,
                TotalAmount = totalAmount,
                SupplierInvoiceID = returnConfirmDto.SupplierInvoiceID
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
                returnConfirmDto.IsPayment);

            if (message.Contains("Success"))
            {
                for (int i = 0; i < purchaseDetails.Count(); i++)
                {
                    foreach (var productID in returnConfirmDto.ProductIDs)
                    {
                        if (productID == list[i].ProductID && returnConfirmDto.ReturnQty[i] > 0)
                        {
                            var returnProductDetails = new SupplierReturnInvoiceDetail()
                            {
                                SupplierInvoiceID = returnConfirmDto.SupplierInvoiceID,
                                PurchaseReturnQuantity = returnConfirmDto.ReturnQty[i],
                                ProductID = productID,
                                PurchaseReturnUnitPrice = list[i].PurchaseUnitPrice,
                                SupplierReturnInvoiceID = returnInvoiceHeader.SupplierReturnInvoiceID,
                                SupplierInvoiceDetailID = list[i].SupplierInvoiceDetailID
                            };

                            await _purchaseReturnFacade.SupplierReturnInvoiceDetailRepository.AddAsync(returnProductDetails);

                            var stock = await _purchaseReturnFacade.StockRepository.GetByIdAsync(productID);
                            if (stock != null)
                            {
                                stock.Quantity -= returnConfirmDto.ReturnQty[i];
                                await _purchaseReturnFacade.StockRepository.UpdateAsync(stock);
                            }
                        }
                    }
                }

                return (true, "Return Successfully", invoiceNo);
            }

            return (false, "Unexpected Issue", invoiceNo);
        }
    }
}
