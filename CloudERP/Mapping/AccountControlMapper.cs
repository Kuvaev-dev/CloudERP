using CloudERP.Models;
using Domain.Models;

namespace CloudERP.Mapping
{
    public class AccountControlMapper
    {
        public static AccountControl MapToDomain(AccountControlMV viewModel)
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

        public static AccountControlMV MapToViewModel(AccountControl domainModel)
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