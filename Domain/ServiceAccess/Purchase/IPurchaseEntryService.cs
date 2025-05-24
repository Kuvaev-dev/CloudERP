using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface IPurchaseEntryService
    {
        Task<string> ConfirmPurchase(int companyID, int branchID, int userID, string invoiceNo, string supplierInvoiceID, float amount, string supplierID, string supplierName, bool isPayment);
        Task<string> PurchasePayment(int companyID, int branchID, int userID, string invoiceNo, string supplierInvoiceID, float totalAmount, float amount, string supplierID, string supplierName, float remainingBalance);
        Task<string> ReturnPurchase(int companyID, int branchID, int userID, string invoiceNo, string supplierInvoiceID, int supplierReturnInvoiceID, float amount, string supplierID, string supplierName, bool isPayment);
        Task<string> ReturnPurchasePayment(int companyID, int branchID, int userID, string invoiceNo, string supplierInvoiceID, int supplierReturnInvoiceID, float totalAmount, float amount, string supplierID, string supplierName, float remainingBalance);
        Task CompletePurchase(IEnumerable<PurchaseCartDetail> purchaseDetails);
    }
}