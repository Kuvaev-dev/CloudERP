namespace Domain.RepositoryAccess
{
    public interface ICustomerPaymentRepository
    {
        Task<double> GetTotalPaidAmountById(int id);
    }
}
