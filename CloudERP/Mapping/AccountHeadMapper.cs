using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;

namespace CloudERP.Mapping
{
    public class AccountHeadMapper : BaseMapper<AccountHead, AccountHeadMV>
    {
        public override AccountHead MapToDomain(AccountHeadMV viewModel)
        {
            return new AccountHead
            {
                AccountHeadID = viewModel.AccountHeadID,
                AccountHeadName = viewModel.AccountHeadName,
                Code = viewModel.Code,
                UserID = viewModel.UserID
            };
        }

        public override AccountHeadMV MapToViewModel(AccountHead domainModel)
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