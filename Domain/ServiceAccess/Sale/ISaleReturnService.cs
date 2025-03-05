using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface ISaleReturnService
    {
        Task<SaleReturnConfirmResult> ProcessReturnConfirmAsync(SaleReturnConfirm returnConfirmDto);
    }
}
