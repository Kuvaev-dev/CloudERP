using Domain.Facades;
using Domain.Models;
using Domain.Models.FinancialModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IPurchaseCartService
    {
        Task<string> ConfirmPurchase(PurchaseConfirm dto, int companyId, int branchId, int userId);
    }

    public class PurchaseCartService : IPurchaseCartService
    {
        private readonly PurchaseCartFacade _purchaseCartFacade;

        public PurchaseCartService(PurchaseCartFacade purchaseCartFacade)
        {
            _purchaseCartFacade = purchaseCartFacade;
        }

        public async Task<string> ConfirmPurchase(PurchaseConfirm dto, int companyId, int branchId, int userId)
        {
            int supplierID = dto.SupplierId;
            string description = dto.Description;
            bool isPayment = dto.IsPayment;

            double totalAmount = dto.PurchaseDetails.Sum(item => item.PurchaseQuantity * item.PurchaseUnitPrice);

            string invoiceNo = GenerateInvoiceNumber();
            var invoiceHeader = CreateInvoiceHeader(companyId, branchId, userId, supplierID, description, totalAmount, invoiceNo);
            await _purchaseCartFacade.SupplierInvoiceRepository.AddAsync(invoiceHeader);

            await AddInvoiceDetails(dto.PurchaseDetails, invoiceHeader.SupplierInvoiceID);

            string message = await _purchaseCartFacade.PurchaseEntryService.ConfirmPurchase(
                companyId,
                branchId,
                userId,
                invoiceNo,
                invoiceHeader.SupplierInvoiceID.ToString(),
                (float)totalAmount,
                supplierID.ToString(),
                (await _purchaseCartFacade.SupplierRepository.GetByIdAsync(supplierID)).SupplierName,
                isPayment);

            if (message.Contains("Success"))
            {
                await UpdateStockAndClearCart(dto.PurchaseDetails);
                return invoiceHeader.SupplierInvoiceID.ToString();
            }

            return message;
        }

        private string GenerateInvoiceNumber()
        {
            return "PUR" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
        }

        private SupplierInvoice CreateInvoiceHeader(int companyId, int branchId, int userId, int supplierID, string description, double totalAmount, string invoiceNo)
        {
            return new SupplierInvoice()
            {
                BranchID = companyId,
                CompanyID = branchId,
                Description = description,
                InvoiceDate = DateTime.Now,
                InvoiceNo = invoiceNo,
                SupplierID = supplierID,
                UserID = userId,
                TotalAmount = totalAmount
            };
        }

        private async Task AddInvoiceDetails(IEnumerable<PurchaseCartDetail> purchaseDetails, int invoiceID)
        {
            foreach (var item in purchaseDetails)
            {
                var newPurchaseDetails = new SupplierInvoiceDetail()
                {
                    ProductID = item.ProductID,
                    PurchaseQuantity = item.PurchaseQuantity,
                    PurchaseUnitPrice = item.PurchaseUnitPrice,
                    SupplierInvoiceID = invoiceID
                };
                await _purchaseCartFacade.SupplierInvoiceDetailRepository.AddAsync(newPurchaseDetails);
            }
        }

        private async Task UpdateStockAndClearCart(IEnumerable<PurchaseCartDetail> purchaseDetails)
        {
            foreach (var item in purchaseDetails)
            {
                var stockItem = await _purchaseCartFacade.StockRepository.GetByIdAsync(item.ProductID);
                if (stockItem != null)
                {
                    stockItem.CurrentPurchaseUnitPrice = item.PurchaseUnitPrice;
                    stockItem.Quantity += item.PurchaseQuantity;
                    await _purchaseCartFacade.StockRepository.UpdateAsync(stockItem);
                }
                await _purchaseCartFacade.PurchaseCartDetailRepository.DeleteAsync(item);
            }
        }
    }
}
