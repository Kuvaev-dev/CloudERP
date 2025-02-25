using Domain.Models;
using System.Data;

namespace Domain.RepositoryAccess
{
    public interface ISaleRepository
    {
        void SetEntries(DataTable dataTable);

        Task<List<SaleInfo>> RemainingPaymentList(int CompanyID, int BranchID);
        Task<List<SaleInfo>> CustomSalesList(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate);
        Task<List<SaleInfo>> SalePaymentHistory(int CustomerInvoiceID);
        Task<List<SaleInfo>> GetReturnSaleAmountPending(int CompanyID, int BranchID);

        Task<string> ConfirmSale(int CompanyID, int BranchID, int UserID, string CustomerInvoiceID, float Amount, string CustomerID, string payInvoiceNo, float RemainingBalance);
        Task<string> ReturnSale(int CompanyID, int BranchID, int UserID, string CustomerInvoiceID, int CustomerReturnInvoiceID, float Amount, string CustomerID, string payInvoiceNo, float RemainingBalance);
        Task<string> InsertTransaction(int CompanyID, int BranchID);
        Task ReturnSalePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, int CustomerReturnInvoiceID, float TotalAmount, float Amount, string CustomerID, float RemainingBalance);
    }
}
