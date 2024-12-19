using DatabaseAccess.Repositories;
using DatabaseAccess;
using Domain.Mapping.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using System;

namespace Domain.Services
{
    public interface IUserService
    {
        Task<IEnumerable<Models.User>> GetAllAsync();
        Task<IEnumerable<Models.User>> GetByBranchAsync(int companyId, int branchTypeId, int branchId);
        Task<Models.User> GetByEmailAsync(string email);
        Task<Models.User> GetByPasswordCodesAsync(string id, DateTime expiration);
        Task<Models.User> GetByIdAsync(int id);
        Task CreateAsync(Models.User user);
        Task UpdateAsync(Models.User user);
        Task DeleteAsync(int id);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper<Models.User, tblUser> _mapper;

        public UserService(IUserRepository repository, IMapper<Models.User, tblUser> mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Models.User>> GetAllAsync()
        {
            var users = await _repository.GetAllAsync();
            return users.Select(_mapper.MapToDomain);
        }

        public async Task<IEnumerable<Models.User>> GetByBranchAsync(int companyId, int branchTypeId, int branchId)
        {
            var users = await _repository.GetByBranchAsync(companyId, branchTypeId, branchId);
            return users.Select(_mapper.MapToDomain);
        }

        public async Task<Models.User> GetByIdAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            return user == null ? null : _mapper.MapToDomain(user);
        }

        public async Task CreateAsync(Models.User user)
        {
            var dbModel = _mapper.MapToDatabase(user);
            await _repository.AddAsync(dbModel);
        }

        public async Task UpdateAsync(Models.User user)
        {
            var dbModel = _mapper.MapToDatabase(user);
            await _repository.UpdateAsync(dbModel);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var user = await _repository.GetByEmailAsync(email);
            return user == null ? null : _mapper.MapToDomain(user);
        }

        public async Task<User> GetByPasswordCodesAsync(string id, DateTime expiration)
        {
            var user = await _repository.GetByPasswordCodesAsync(id, expiration);
            return user == null ? null : _mapper.MapToDomain(user);
        }
    }
}
