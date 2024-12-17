using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;

namespace CloudERP.Mapping
{
    public class AccountSettingMapper : BaseMapper<AccountSetting, AccountSettingMV>
    {
        public override AccountSetting MapToDomain(AccountSettingMV viewModel)
        {
            return new AccountSetting
            {
                AccountSettingID = viewModel.AccountSettingID,
                AccountHeadID = viewModel.AccountHeadID,
                AccountHeadName = viewModel.AccountHeadName,
                AccountControlID = viewModel.AccountControlID,
                AccountControlName = viewModel.AccountControlName,
                AccountSubControlID = viewModel.AccountSubControlID,
                AccountSubControlName = viewModel.AccountSubControlName,
                AccountActivityID = viewModel.AccountActivityID,
                AccountActivityName = viewModel.AccountActivityName,
                CompanyID = viewModel.CompanyID,
                BranchID = viewModel.BranchID
            };
        }

        public override AccountSettingMV MapToViewModel(AccountSetting domainModel)
        {
            return new AccountSettingMV
            {
                AccountSettingID = domainModel.AccountSettingID,
                AccountHeadID = domainModel.AccountHeadID,
                AccountHeadName = domainModel.AccountHeadName,
                AccountControlID = domainModel.AccountControlID,
                AccountControlName = domainModel.AccountControlName,
                AccountSubControlID = domainModel.AccountSubControlID,
                AccountSubControlName = domainModel.AccountSubControlName,
                AccountActivityID = domainModel.AccountActivityID,
                AccountActivityName = domainModel.AccountActivityName,
                CompanyID = domainModel.CompanyID,
                BranchID = domainModel.BranchID
            };
        }
    }
}