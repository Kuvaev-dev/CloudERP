using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface ISaleEntryService
    {
        Task<string> ConfirmSale(SaleConfirm saleConfirm, string invoiceNo, string customerInvoiceID, float amount, string customerID, string customerName);
        Task<string> SalePayment(int companyID, int branchID, int userID, string invoiceNo, string customerInvoiceID, float totalAmount, float amount, string customerID, string customerName, float remainingBalance);
        Task<string> ReturnSale(int companyID, int branchID, int userID, string invoiceNo, string customerInvoiceID, int customerReturnInvoiceID, float amount, string customerID, string customerName, bool isPayment);
        Task<string> ReturnSalePayment(int companyID, int branchID, int userID, string invoiceNo, string customerInvoiceID, int customerReturnInvoiceID, float totalAmount, float amount, string customerID, string customerName, float remainingBalance);
        Task CompleteSale(IEnumerable<SaleCartDetail> saleDetails);
    }
}