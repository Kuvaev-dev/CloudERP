﻿using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<IEnumerable<Customer>> GetByCompanyAndBranchAsync(int companyId, int branchId);
        Task<IEnumerable<Customer>> GetByBranchesAsync(int branchId);
        Task<Customer> GetByIdAsync(int id);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task<bool> IsExists(Customer customer);
    }
}
