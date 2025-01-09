using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface IUserTypeRepository
    {
        Task<IEnumerable<UserType>> GetAllAsync();
        Task<UserType> GetByIdAsync(int id);
        Task AddAsync(UserType userType);
        Task UpdateAsync(UserType userType);
    }
}
