using System.Threading.Tasks;

namespace Domain.ServiceAccess
{
    public interface ISalaryTransactionService
    {
        Task<string> Confirm(
            int EmployeeID,
            double TransferAmount,
            int UserID,
            int BranchID,
            int CompanyID,
            string InvoiceNo,
            string SalaryMonth,
            string SalaryYear);
    }
}
