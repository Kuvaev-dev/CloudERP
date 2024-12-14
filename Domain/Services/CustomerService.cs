using DatabaseAccess.Repositories;
using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudERP.Helpers;
using System.Linq;

namespace Domain.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<IEnumerable<Customer>> GetCustomersByCompanyAndBranchAsync(int companyId, int branchId);
        Task<IEnumerable<Customer>> GetSubBranchCustomersAsync(int branchId);
        Task<Customer> GetCustomerByIdAsync(int id);
        Task CreateCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int id);
    }

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IMapper<Customer, tblCustomer> _mapper;

        public CustomerService(ICustomerRepository repository, IMapper<Customer, tblCustomer> mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            var customers = await _repository.GetAllAsync();
            return customers.Select(_mapper.MapToDomain);
        }

        public async Task<IEnumerable<Customer>> GetCustomersByCompanyAndBranchAsync(int companyId, int branchId)
        {
            var customers = await _repository.GetByCompanyAndBranchAsync(companyId, branchId);
            return customers.Select(_mapper.MapToDomain);
        }

        public async Task<IEnumerable<Customer>> GetSubBranchCustomersAsync(int branchId)
        {
            var branchIds = BranchHelper.GetBranchsIDs(branchId, _repository.DbContext);
            var customers = await _repository.GetByBranchesAsync(branchIds);
            return customers.Select(_mapper.MapToDomain);
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);
            return customer == null ? null : _mapper.MapToDomain(customer);
        }

        public async Task CreateCustomerAsync(Customer customer)
        {
            var dbModel = _mapper.MapToDatabase(customer);
            await _repository.AddAsync(dbModel);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            var dbModel = _mapper.MapToDatabase(customer);
            await _repository.UpdateAsync(dbModel);
        }

        public async Task DeleteCustomerAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
