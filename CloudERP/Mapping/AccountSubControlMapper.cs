using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;

namespace CloudERP.Mapping
{
    public class AccountSubControlMapper : BaseMapper<AccountSubControl, AccountSubControlMV>
    {
        public override AccountSubControl MapToDomain(AccountSubControlMV viewModel)
        {
            return new AccountSubControl
            {
                AccountSubControlName = viewModel.AccountSubControlName,
                AccountControlID = viewModel.AccountControlID,
                CompanyID = viewModel.CompanyID,
                BranchID = viewModel.BranchID,
                UserID = viewModel.UserID,
                AccountHeadID = viewModel.AccountHeadID
            };
        }

        public override AccountSubControlMV MapToViewModel(AccountSubControl domainModel)
        {
            return new AccountSubControlMV
            {
                AccountSubControlID = domainModel.AccountSubControlID,
                AccountSubControlName = domainModel.AccountSubControlName,
                AccountControlID = domainModel.AccountControlID,
                CompanyID = domainModel.CompanyID,
                BranchID = domainModel.BranchID,
                UserID = domainModel.UserID,
                AccountControlList = null
            };
        }
    }
}