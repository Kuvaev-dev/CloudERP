namespace Domain.ServiceAccess
{
    public interface IGeneralTransactionService
    {
        Task<string> ConfirmTransactionAsync(float transferAmount, int userId, int branchId, int companyId, int debitAccountControlID, int creditAccountControlID, string reason);
    }
}
