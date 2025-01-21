using Domain.Models.FinancialModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ISaleRepository
    {
        Task<List<SalePaymentModel>> RemainingPaymentList(int CompanyID, int BranchID);
        Task<List<SalePaymentModel>> CustomSalesList(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate);
        Task<List<SalePaymentModel>> SalePaymentHistory(int CustomerInvoiceID);
        Task<List<SalePaymentModel>> GetReturnSaleAmountPending(int CompanyID, int BranchID);

        Task<string> ConfirmSale(int CompanyID, int BranchID, int UserID, string CustomerInvoiceID, float Amount, string CustomerID, string payInvoiceNo, float RemainingBalance);
        Task<string> ReturnSale(int CompanyID, int BranchID, int UserID, string CustomerInvoiceID, int CustomerReturnInvoiceID, float Amount, string CustomerID, string payInvoiceNo, float RemainingBalance);
        Task<string> InsertTransaction(int CompanyID, int BranchID);
        Task ReturnSalePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, int CustomerReturnInvoiceID, float TotalAmount, float Amount, string CustomerID, float RemainingBalance);
    }
}
