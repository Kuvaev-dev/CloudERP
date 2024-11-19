using DatabaseAccess.Repositories;
using DatabaseAccess;
using System.Collections.Generic;
using System.Linq;
using Domain.ViewModels;

namespace Domain.Services
{
    public interface IAccountControlService
    {
        IEnumerable<AccountControlMV> GetAll(int companyId, int branchId);
        AccountControlMV GetById(int id);
        void Create(AccountControlMV accountControl);
        void Update(AccountControlMV accountControl);
    }

    public class AccountControlService : IAccountControlService
    {
        private readonly IAccountControlRepository _repository;

        public AccountControlService(IAccountControlRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<AccountControlMV> GetAll(int companyId, int branchId)
        {
            var entities = _repository.GetAccountControls(companyId, branchId);
            return entities.Select(e => new AccountControlMV
            {
                AccountControlID = e.AccountControlID,
                AccountControlName = e.AccountControlName,
                AccountHeadID = e.AccountHeadID,
                AccountHeadName = e.tblAccountHead?.AccountHeadName,
                BranchID = e.BranchID,
                BranchName = e.tblBranch?.BranchName,
                CompanyID = e.CompanyID,
                Name = e.tblCompany?.Name,
                UserID = e.UserID,
                UserName = e.tblUser?.UserName
            });
        }

        public AccountControlMV GetById(int id)
        {
            var entity = _repository.GetById(id);
            if (entity == null)
                return null;

            return new AccountControlMV
            {
                AccountControlID = entity.AccountControlID,
                AccountControlName = entity.AccountControlName,
                AccountHeadID = entity.AccountHeadID,
                AccountHeadName = entity.tblAccountHead?.AccountHeadName,
                BranchID = entity.BranchID,
                BranchName = entity.tblBranch?.BranchName,
                CompanyID = entity.CompanyID,
                Name = entity.tblCompany?.Name,
                UserID = entity.UserID,
                UserName = entity.tblUser?.UserName
            };
        }

        public void Create(AccountControlMV accountControl)
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

        public void Update(AccountControlMV accountControl)
        {
            var entity = _repository.GetById(accountControl.AccountControlID);
            if (entity == null) throw new KeyNotFoundException("AccountControl not found.");

            entity.AccountControlName = accountControl.AccountControlName;
            entity.AccountHeadID = accountControl.AccountHeadID;

            _repository.Update(entity);
        }
    }
}
