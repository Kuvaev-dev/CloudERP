using CloudERP.Models;
using Domain.Models;

namespace CloudERP.Mapping
{
    public static class AccountHeadMapper
    {
        public static AccountHead MapToDomain(AccountHeadMV viewModel)
        {
            return new AccountHead
            {
                AccountHeadID = viewModel.AccountHeadID,
                AccountHeadName = viewModel.AccountHeadName,
                Code = viewModel.Code,
                UserID = viewModel.UserID
            };
        }

        public static AccountHeadMV MapToViewModel(AccountHead domainModel)
        {
            return new AccountHeadMV
            {
                AccountHeadID = domainModel.AccountHeadID,
                AccountHeadName = domainModel.AccountHeadName,
                Code = domainModel.Code,
                UserID = domainModel.UserID
            };
        }
    }
}