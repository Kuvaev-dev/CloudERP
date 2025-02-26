namespace Domain.RepositoryAccess
{
    public interface ISalaryTransactionRepository
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
        Task<string> InsertTransaction(int CompanyID, int BranchID);
    }
}
