using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;

namespace Domain.Mapping
{
    public class AccountSettingMapper : BaseMapper<AccountSetting, tblAccountSetting>
    {
        public override AccountSetting MapToDomain(tblAccountSetting dbModel)
        {
            return new AccountSetting
            {
                AccountSettingID = dbModel.AccountSettingID,
                AccountHeadID = dbModel.AccountHeadID,
                AccountHeadName = dbModel.tblAccountHead?.AccountHeadName,
                AccountControlID = dbModel.AccountControlID,
                AccountControlName = dbModel.tblAccountControl?.AccountControlName,
                AccountSubControlID = dbModel.AccountSubControlID,
                AccountSubControlName = dbModel.tblAccountSubControl?.AccountSubControlName,
                AccountActivityID = dbModel.AccountActivityID,
                AccountActivityName = dbModel.tblAccountActivity?.Name,
                CompanyID = dbModel.CompanyID,
                CompanyName = dbModel.tblCompany?.Name,
                BranchID = dbModel.BranchID,
                BranchName = dbModel.tblBranch?.BranchName
            };
        }

        public override tblAccountSetting MapToDatabase(AccountSetting domainModel)
        {
            return new tblAccountSetting
            {
                AccountSettingID = domainModel.AccountSettingID,
                AccountHeadID = domainModel.AccountHeadID,
                AccountControlID = domainModel.AccountControlID,
                AccountSubControlID = domainModel.AccountSubControlID,
                AccountActivityID = domainModel.AccountActivityID,
                CompanyID = domainModel.CompanyID,
                BranchID = domainModel.BranchID
            };
        }
    }
}
