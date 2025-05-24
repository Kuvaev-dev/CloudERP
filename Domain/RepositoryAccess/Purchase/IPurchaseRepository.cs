using Domain.Models;
using Domain.Models.FinancialModels;
using System.Data;

namespace Domain.RepositoryAccess
{
    public interface IPurchaseRepository
    {
        Task SetEntries(DataTable dataTable);

        Task<List<PurchaseInfo>> RemainingPaymentList(int CompanyID, int BranchID);
        Task<List<PurchaseInfo>> CustomPurchasesList(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate);
        Task<List<PurchaseInfo>> PurchasePaymentHistory(int SupplierInvoiceID);
        Task<List<SupplierReturnInvoiceModel>> PurchaseReturnPaymentPending(int? SupplierInvoiceID);
        Task<List<PurchaseInfo>> GetReturnPurchasesPaymentPending(int CompanyID, int BranchID);

        Task<string> ConfirmPurchase(int CompanyID, int BranchID, int UserID, string SupplierInvoiceID, float Amount, string SupplierID, string payInvoiceNo, float RemainingBalance);
        Task<string> ConfirmPurchaseReturn(int CompanyID, int BranchID, int UserID, string SupplierInvoiceID, int SupplierReturnInvoiceID, float Amount, string SupplierID, string payInvoiceNo);
        Task<string> InsertTransaction(int CompanyID, int BranchID);
    }
}
