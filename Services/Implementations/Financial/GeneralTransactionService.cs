using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace Services.Implementations
{
    public class GeneralTransactionService : IGeneralTransactionService
    {
        private readonly IGeneralTransactionRepository _generalTransactionRepository;

        public GeneralTransactionService(IGeneralTransactionRepository generalTransactionRepository)
        {
            _generalTransactionRepository = generalTransactionRepository ?? throw new ArgumentNullException(nameof(IGeneralTransactionRepository));
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
