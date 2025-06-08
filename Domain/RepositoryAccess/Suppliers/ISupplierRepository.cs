using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface ISupplierRepository
    {
        Task<IEnumerable<Supplier>> GetAllAsync();
        Task<IEnumerable<Supplier>> GetByCompanyAndBranchAsync(int companyID, int branchID);
        Task<Supplier> GetByIdAsync(int id);
        Task<Supplier> GetByNameAndContactAsync(int companyID, int branchID, string supplierName, string contactNo);
        Task<IEnumerable<Supplier>> GetSuppliersByBranchesAsync(int branchID);
        Task AddAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task<bool> IsExists(Supplier supplier);
    }
}
