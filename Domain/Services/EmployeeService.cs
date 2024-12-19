using DatabaseAccess.Repositories;
using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetEmployeesByBranchAsync(int companyId, int branchId);
        Task<Employee> GetByIdAsync(int id);
        Task<Employee> GetByUserIdAsync(int id);
        Task CreateAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task<bool> IsFirstLoginAsync(Employee employee);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper<Employee, tblEmployee> _mapper;

        public EmployeeService(IEmployeeRepository repository, IMapper<Employee, tblEmployee> mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByBranchAsync(int companyId, int branchId)
        {
            var employees = await _repository.GetByBranchAsync(companyId, branchId);
            return employees.Select(_mapper.MapToDomain);
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            var employee = await _repository.GetByIdAsync(id);
            return employee == null ? null : _mapper.MapToDomain(employee);
        }

        public async Task CreateAsync(Employee employee)
        {
            var dbModel = _mapper.MapToDatabase(employee);
            await _repository.AddAsync(dbModel);
        }

        public async Task UpdateAsync(Employee employee)
        {
            var dbModel = _mapper.MapToDatabase(employee);
            await _repository.UpdateAsync(dbModel);
        }

        public async Task<Employee> GetByUserIdAsync(int id)
        {
            var employee = await _repository.GetByUserIdAsync(id);
            return employee == null ? null : _mapper.MapToDomain(employee);
        }

        public async Task<bool> IsFirstLoginAsync(Employee employee)
        {
            return await _repository.IsFirstLoginAsync(_mapper.MapToDatabase(employee));
        }
    }
}
