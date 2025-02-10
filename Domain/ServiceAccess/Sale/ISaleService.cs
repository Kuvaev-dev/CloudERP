using Domain.Models.Sale;
using System.Threading.Tasks;

namespace Domain.ServiceAccess
{
    public interface ISaleService
    {
        Task<SaleItemDetailDto> GetSaleItemDetailAsync(int id);
    }
}
