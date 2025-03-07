using Domain.Facades;
using Domain.Models;
using Domain.ServiceAccess;
using Utils.Helpers;

namespace Services.Implementations
{
    public class PurchaseCartService : IPurchaseCartService
    {
        private readonly PurchaseCartFacade _purchaseCartFacade;

        public PurchaseCartService(PurchaseCartFacade purchaseCartFacade)
        {
            _purchaseCartFacade = purchaseCartFacade ?? throw new ArgumentNullException(nameof(PurchaseCartFacade));
        }

        public async Task<Result<int>> ConfirmPurchaseAsync(PurchaseConfirm dto)
        {
            try
            {
                var supplier = await _purchaseCartFacade.SupplierRepository.GetByIdAsync(dto.SupplierId);
                var purchaseDetails = await _purchaseCartFacade.PurchaseCartDetailRepository.GetAllAsync(dto.BranchID, dto.CompanyID);

                double totalAmount = purchaseDetails.Sum(item => item.PurchaseQuantity * item.PurchaseUnitPrice);

                string invoiceNo = "PUR" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var invoiceHeader = new SupplierInvoice
                {
                    BranchID = dto.BranchID,
                    Title = $"{Localization.Services.Localization.PurchaseInvoice}: {supplier.SupplierName}",
                    CompanyID = dto.CompanyID,
                    Description = dto.Description,
                    InvoiceDate = DateTime.Now,
                    InvoiceNo = invoiceNo,
                    SupplierID = dto.SupplierId,
                    UserID = dto.UserID,
                    TotalAmount = totalAmount
                };

                await _purchaseCartFacade.SupplierInvoiceRepository.AddAsync(invoiceHeader);
                await _purchaseCartFacade.SupplierInvoiceDetailRepository.AddPurchaseDetailsAsync(purchaseDetails, invoiceHeader.SupplierInvoiceID);

                string message = await _purchaseCartFacade.PurchaseEntryService.ConfirmPurchase(
                    dto.CompanyID,
                    dto.BranchID,
                    dto.UserID,
                    invoiceNo,
                    invoiceHeader.SupplierInvoiceID.ToString(),
                    (float)totalAmount,
                    dto.SupplierId.ToString(),
                    supplier.SupplierName,
                    dto.IsPayment);

                if (message.Contains("Success"))
                {
                    await _purchaseCartFacade.PurchaseEntryService.CompletePurchase(purchaseDetails);
                    return Result<int>.Success(invoiceHeader.SupplierInvoiceID);
                }

                return Result<int>.Failure(message);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure("Unexpected Issue Occurred: " + ex.Message);
            }
        }

        private static string GenerateInvoiceNumber()
        {
            return "PUR" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
        }

        private static SupplierInvoice CreateInvoiceHeader(int companyId, int branchId, int userId, int supplierID, string description, double totalAmount, string invoiceNo)
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
                await _purchaseCartFacade.PurchaseCartDetailRepository.DeleteAsync(item.PurchaseCartDetailID);
            }
        }
    }
}
