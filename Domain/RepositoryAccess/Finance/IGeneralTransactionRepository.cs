using Domain.Models.FinancialModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface IGeneralTransactionRepository
    {
        Task<string> ConfirmGeneralTransaction(
            float transferAmount,
            int userId,
            int branchId,
            int companyId,
            string invoiceNo,
            int debitAccountControlId,
            int creditAccountControlId,
            string reason);
        Task<List<AllAccountModel>> GetAllAccounts(int CompanyID, int BranchID);
        Task<List<JournalModel>> GetJournal(int CompanyID, int? BranchID, DateTime FromDate, DateTime ToDate);
    }
}
