using DatabaseAccess;
using DatabaseAccess.Repositories;
using Domain.Mapping.Base;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IUserTypeService
    {
        Task<IEnumerable<UserType>> GetAllAsync();
        Task<UserType> GetByIdAsync(int id);
        Task CreateAsync(UserType userType);
        Task UpdateAsync(UserType userType);
        Task DeleteAsync(int id);
    }

    public class UserTypeService : IUserTypeService
    {
        private readonly IUserTypeRepository _repository;
        private readonly IMapper<UserType, tblUserType> _mapper;

        public UserTypeService(IUserTypeRepository repository, IMapper<UserType, tblUserType> mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserType>> GetAllAsync()
        {
            var dbModels = await _repository.GetAllAsync();
            var userTypes = dbModels.Select(_mapper.MapToDomain).ToList();

            return userTypes;
        }

        public async Task<UserType> GetByIdAsync(int id)
        {
            var dbModel = await _repository.GetByIdAsync(id);
            return dbModel == null ? null : _mapper.MapToDomain(dbModel);
        }

        public async Task CreateAsync(UserType userType)
        {
            var dbModel = _mapper.MapToDatabase(userType);
            await _repository.AddAsync(dbModel);
        }

        public async Task UpdateAsync(UserType userType)
        {
            var dbModel = _mapper.MapToDatabase(userType);
            await _repository.UpdateAsync(dbModel);
        }

        public async Task DeleteAsync(int id)
        {
            var dbModel = await _repository.GetByIdAsync(id);
            if (dbModel == null) throw new KeyNotFoundException("UserType not found.");
            await _repository.DeleteAsync(dbModel);
        }
    }
}
