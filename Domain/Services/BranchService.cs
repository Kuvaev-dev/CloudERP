using DatabaseAccess;
using DatabaseAccess.Repositories;
using Domain.Mapping.Base;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IBranchService
    {
        Task<IEnumerable<Branch>> GetBranchesByFilterAsync(int companyId, int branchTypeId, int branchId);
        Task<IEnumerable<Branch>> GetSubBranchesAsync(int companyId, int branchId);
        Task<Branch> GetByIdAsync(int id);
        Task<IEnumerable<Branch>> GetBranchesByCompanyAsync(int companyId);
        Task<IEnumerable<string>> GetBranchTypesAsync();
        Task CreateAsync(Branch branch);
        Task UpdateAsync(Branch branch);
        Task DeleteAsync(int id);
        Task<bool> CheckBranchExistsAsync(Branch branch, bool isEdit = false);
    }

    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _repository;
        private readonly IMapper<Branch, tblBranch> _mapper;

        public BranchService(IBranchRepository repository, IMapper<Branch, tblBranch> mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Branch>> GetBranchesByFilterAsync(int companyId, int branchTypeId, int branchId)
        {
            var branches = branchTypeId == 1
                ? await _repository.GetByCompanyAsync(companyId)
                : await _repository.GetSubBranchAsync(companyId, branchId);

            return branches.Select(_mapper.MapToDomain);
        }

        public async Task<IEnumerable<Branch>> GetSubBranchesAsync(int companyId, int branchId)
        {
            var branches = await _repository.GetSubBranchAsync(companyId, branchId);
            return branches.Select(_mapper.MapToDomain);
        }

        public async Task<Branch> GetByIdAsync(int id)
        {
            var branch = await _repository.GetByIdAsync(id);
            return branch == null ? null : _mapper.MapToDomain(branch);
        }

        public async Task<IEnumerable<Branch>> GetBranchesByCompanyAsync(int companyId)
        {
            var branches = await _repository.GetByCompanyAsync(companyId);
            return branches.Select(_mapper.MapToDomain);
        }

        public Task<IEnumerable<string>> GetBranchTypesAsync()
        {
            return Task.FromResult(new List<string> { "Main Branch", "Sub Branch" }.AsEnumerable());
        }

        public async Task CreateAsync(Branch branch)
        {
            var dbModel = _mapper.MapToDatabase(branch);
            await _repository.AddAsync(dbModel);
        }

        public async Task UpdateAsync(Branch branch)
        {
            var dbModel = _mapper.MapToDatabase(branch);
            await _repository.UpdateAsync(dbModel);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<bool> CheckBranchExistsAsync(Branch branch, bool isEdit = false)
        {
            var branches = await _repository.GetByCompanyAsync(branch.CompanyID);
            return branches.Any(b =>
                b.BranchName == branch.BranchName ||
                b.BranchContact == branch.BranchContact ||
                b.BranchAddress == branch.BranchAddress &&
                (!isEdit || b.BranchID != branch.BranchID));
        }
    }
}
