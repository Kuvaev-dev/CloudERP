using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;

namespace CloudERP.Mapping
{
    public class AccountControlMapper : BaseMapper<AccountControl, AccountControlMV>
    {
        public override AccountControl MapToDomain(AccountControlMV viewModel)
        {
            return new AccountControl
            {
                AccountControlID = viewModel.AccountControlID,
                AccountControlName = viewModel.AccountControlName,
                AccountHeadID = viewModel.AccountHeadID,
                BranchID = viewModel.BranchID,
                CompanyID = viewModel.CompanyID,
                UserID = viewModel.UserID
            };
        }

        public override AccountControlMV MapToViewModel(AccountControl domainModel)
        {
            return new AccountControlMV
            {
                AccountControlID = domainModel.AccountControlID,
                AccountControlName = domainModel.AccountControlName,
                AccountHeadID = domainModel.AccountHeadID,
                BranchID = domainModel.BranchID,
                CompanyID = domainModel.CompanyID,
                UserID = domainModel.UserID,
                AccountHeadList = null
            };
        }
    }
}