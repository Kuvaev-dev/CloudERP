using DatabaseAccess.Repositories;
using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface ICompanyService
    {
        Task<IEnumerable<Company>> GetAllAsync();
        Task<Company> GetByIdAsync(int id);
        Task CreateAsync(Company company);
        Task UpdateAsync(Company company);
        Task<bool> CheckCompanyExistsAsync(string name);
    }

    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _repository;
        private readonly IMapper<Company, tblCompany> _mapper;

        public CompanyService(ICompanyRepository repository, IMapper<Company, tblCompany> mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            var companies = await _repository.GetAllAsync();
            return companies.Select(_mapper.MapToDomain);
        }

        public async Task<Company> GetByIdAsync(int id)
        {
            var company = await _repository.GetByIdAsync(id);
            return company == null ? null : _mapper.MapToDomain(company);
        }

        public async Task CreateAsync(Company company)
        {
            var dbModel = _mapper.MapToDatabase(company);
            await _repository.AddAsync(dbModel);
        }

        public async Task UpdateAsync(Company company)
        {
            var dbModel = _mapper.MapToDatabase(company);
            await _repository.UpdateAsync(dbModel);
        }

        public async Task<bool> CheckCompanyExistsAsync(string name)
        {
            var companies = await _repository.GetAllAsync();
            return companies.Any(c => c.Name == name);
        }
    }
}
