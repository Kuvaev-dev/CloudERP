using DatabaseAccess.Repositories;
using Domain.Mapping;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Services
{
    public interface IUserTypeService
    {
        IEnumerable<UserType> GetAll();
        UserType GetById(int id);
        void Create(UserType userType);
        void Update(UserType userType);
        void Delete(int id);
    }

    public class UserTypeService : IUserTypeService
    {
        private readonly IUserTypeRepository _repository;

        public UserTypeService(IUserTypeRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<UserType> GetAll()
        {
            return _repository.GetAll().Select(UserTypeMapper.MapToDomain).ToList();
        }

        public UserType GetById(int id)
        {
            var dbModel = _repository.GetById(id);
            return dbModel == null ? null : UserTypeMapper.MapToDomain(dbModel);
        }

        public void Create(UserType userType)
        {
            var dbModel = UserTypeMapper.MapToDatabase(userType);
            _repository.Add(dbModel);
        }

        public void Update(UserType userType)
        {
            var dbModel = UserTypeMapper.MapToDatabase(userType);
            _repository.Update(dbModel);
        }

        public void Delete(int id)
        {
            var dbModel = _repository.GetById(id);
            if (dbModel == null) throw new KeyNotFoundException("UserType not found.");
            _repository.Delete(dbModel);
        }
    }
}
