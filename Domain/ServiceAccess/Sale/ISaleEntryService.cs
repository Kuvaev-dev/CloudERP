using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface ISaleEntryService
    {
        Task<string> ConfirmSale(
            int CompanyID,
            int BranchID,
            int UserID,
            string InvoiceNo,
            string CustomerInvoiceID,
            float Amount,
            string CustomerID,
            string CustomerName,
            bool isPayment);
        Task<string> SalePayment(
            int CompanyID,
            int BranchID,
            int UserID,
            string InvoiceNo,
            string CustomerInvoiceID,
            float TotalAmount,
            float Amount,
            string CustomerID,
            string CustomerName,
            float RemainingBalance);
        Task<string> ReturnSale(
            int CompanyID,
            int BranchID,
            int UserID,
            string InvoiceNo,
            string CustomerInvoiceID,
            int CustomerReturnInvoiceID,
            float Amount,
            string CustomerID,
            string Customername,
            bool isPayment);
        Task<string> ReturnSalePayment(
            int CompanyID,
            int BranchID,
            int UserID,
            string InvoiceNo,
            string CustomerInvoiceID,
            int CustomerReturnInvoiceID,
            float TotalAmount,
            float Amount,
            string CustomerID,
            string Customername,
            float RemainingBalance);
        Task CompleteSale(IEnumerable<SaleCartDetail> saleDetails);
    }
}
