﻿using Domain.Facades;
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

        public async Task<PurchaseReturnConfirmResult> ProcessReturnAsync(PurchaseReturnConfirm returnConfirmDto)
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
                return new PurchaseReturnConfirmResult { IsSuccess = false, Message = Localization.Services.Localization.OneProductReturnQtyError, InvoiceNo = string.Empty };
            }

            string invoiceNo = "RPU" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
            var returnInvoiceHeader = new SupplierReturnInvoice()
            {
                BranchID = returnConfirmDto.BranchID,
                CompanyID = returnConfirmDto.CompanyID,
                Description = Localization.Services.Localization.PurchaseReturn,
                InvoiceDate = DateTime.Now,
                InvoiceNo = invoiceNo,
                SupplierID = supplierID,
                UserID = returnConfirmDto.UserID,
                TotalAmount = totalAmount,
                SupplierInvoiceID = returnConfirmDto.SupplierInvoiceID
            };

            await _purchaseReturnFacade.SupplierReturnInvoiceRepository.AddAsync(returnInvoiceHeader);

            var supplier = await _purchaseReturnFacade.SupplierRepository.GetByIdAsync(supplierID);
            string message = await _purchaseReturnFacade.PurchaseEntryService.ReturnPurchase(
                returnConfirmDto.CompanyID,
                returnConfirmDto.BranchID,
                returnConfirmDto.UserID,
                invoiceNo,
                returnInvoiceHeader.SupplierInvoiceID.ToString(),
                returnInvoiceHeader.SupplierReturnInvoiceID,
                (float)totalAmount,
                supplierID.ToString(),
                supplier.SupplierName,
                returnConfirmDto.IsPayment);

            if (message.Contains(Localization.Services.Localization.Success))
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

                return new PurchaseReturnConfirmResult { IsSuccess = true, Message = Localization.Services.Localization.ReturnSuccessfully, InvoiceNo = invoiceNo };
            }

            return new PurchaseReturnConfirmResult { IsSuccess = false, Message = Localization.Services.Localization.UnexpectedIssue, InvoiceNo = invoiceNo };
        }
    }
}
