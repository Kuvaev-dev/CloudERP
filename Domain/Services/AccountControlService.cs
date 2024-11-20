using DatabaseAccess.Repositories;
using DatabaseAccess;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;

namespace Domain.Services
{
    public interface IAccountControlService
    {
        IEnumerable<AccountControl> GetAll(int companyId, int branchId);
        AccountControl GetById(int id);
        void Create(AccountControl accountControl);
        void Update(AccountControl accountControl);
    }

    public class AccountControlService : IAccountControlService
    {
        private readonly IAccountControlRepository _repository;

        public AccountControlService(IAccountControlRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<AccountControl> GetAll(int companyId, int branchId)
        {
            var entities = _repository.GetAccountControls(companyId, branchId);
            return entities.Select(e => new AccountControl
            {
                AccountControlID = e.AccountControlID,
                AccountControlName = e.AccountControlName,
                AccountHeadID = e.AccountHeadID,
                AccountHeadName = e.tblAccountHead?.AccountHeadName,
                BranchID = e.BranchID,
                BranchName = e.tblBranch?.BranchName,
                CompanyID = e.CompanyID,
                CompanyName = e.tblCompany?.Name,
                UserID = e.UserID,
                UserName = e.tblUser?.UserName
            });
        }

        public AccountControl GetById(int id)
        {
            var entity = _repository.GetById(id);
            if (entity == null)
                return null;

            return new AccountControl
            {
                AccountControlID = entity.AccountControlID,
                AccountControlName = entity.AccountControlName,
                AccountHeadID = entity.AccountHeadID,
                AccountHeadName = entity.tblAccountHead?.AccountHeadName,
                BranchID = entity.BranchID,
                BranchName = entity.tblBranch?.BranchName,
                CompanyID = entity.CompanyID,
                CompanyName = entity.tblCompany?.Name,
                UserID = entity.UserID,
                UserName = entity.tblUser?.UserName
            };
        }

        public void Create(AccountControl accountControl)
        {
            var entity = new tblAccountControl
            {
                AccountControlName = accountControl.AccountControlName,
                CompanyID = accountControl.CompanyID,
                BranchID = accountControl.BranchID,
                UserID = accountControl.UserID,
                AccountHeadID = accountControl.AccountHeadID
            };

            _repository.Add(entity);
        }

        public void Update(AccountControl accountControl)
        {
            var entity = _repository.GetById(accountControl.AccountControlID);
            if (entity == null) throw new KeyNotFoundException("AccountControl not found.");

            entity.AccountControlName = accountControl.AccountControlName;
            entity.AccountHeadID = accountControl.AccountHeadID;

            _repository.Update(entity);
        }
    }
}
