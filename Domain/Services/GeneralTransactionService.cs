using Domain.RepositoryAccess;
using System;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IGeneralTransactionService
    {
        Task<string> ConfirmTransactionAsync(float transferAmount, int userId, int branchId, int companyId, int debitAccountControlID, int creditAccountControlID, string reason);
    }

    public class GeneralTransactionService : IGeneralTransactionService
    {
        private readonly IGeneralTransactionRepository _generalTransactionRepository;

        public GeneralTransactionService(IGeneralTransactionRepository generalTransactionRepository)
        {
            _generalTransactionRepository = generalTransactionRepository;
        }

        public async Task<string> ConfirmTransactionAsync(
            float transferAmount, 
            int userId, 
            int branchId, 
            int companyId, 
            int debitAccountControlID, 
            int creditAccountControlID, 
            string reason)
        {
            string payInvoiceNo = "GEN" + DateTime.Now.ToString("yyyyMMddHHmmssmm");
            return await _generalTransactionRepository.ConfirmGeneralTransaction(
                transferAmount,
                userId,
                branchId,
                companyId,
                payInvoiceNo,
                debitAccountControlID,
                creditAccountControlID,
                reason);
        }
    }
}
