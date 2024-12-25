using Domain.Models.FinancialModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface IPurchaseRepository
    {
        Task<List<PurchasePaymentModel>> RemainingPaymentList(int CompanyID, int BranchID);
        Task<List<PurchasePaymentModel>> CustomPurchasesList(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate);
        Task<List<PurchasePaymentModel>> PurchasePaymentHistory(int SupplierInvoiceID);
        Task<List<SupplierReturnInvoiceModel>> PurchaseReturnPaymentPending(int? SupplierInvoiceID);
        Task<List<PurchasePaymentModel>> GetReturnPurchasesPaymentPending(int CompanyID, int BranchID);

        Task<string> ConfirmPurchase(int CompanyID, int BranchID, int UserID, string SupplierInvoiceID, float Amount, string SupplierID, string payInvoiceNo, float RemainingBalance);
        Task<string> ConfirmPurchaseReturn(int CompanyID, int BranchID, int UserID, string SupplierInvoiceID, int SupplierReturnInvoiceID, float Amount, string SupplierID, string payInvoiceNo);
        Task<string> InsertTransaction(int CompanyID, int BranchID);
    }
}
