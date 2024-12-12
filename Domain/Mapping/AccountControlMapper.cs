using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;

namespace Domain.Mapping
{
    public class AccountControlMapper : BaseMapper<AccountControl, tblAccountControl>
    {
        public override AccountControl MapToDomain(tblAccountControl dbModel)
        {
            return new AccountControl
            {
                AccountControlID = dbModel.AccountControlID,
                AccountControlName = dbModel.AccountControlName,
                AccountHeadID = dbModel.AccountHeadID,
                AccountHeadName = "Unknown",
                BranchID = dbModel.BranchID,
                CompanyID = dbModel.CompanyID,
                UserID = dbModel.UserID,
                FullName = dbModel.tblUser?.FullName ?? "Unknown"
            };
        }

        public override tblAccountControl MapToDatabase(AccountControl domainModel)
        {
            return new tblAccountControl
            {
                AccountControlID = domainModel.AccountControlID,
                AccountControlName = domainModel.AccountControlName,
                AccountHeadID = domainModel.AccountHeadID,
                BranchID = domainModel.BranchID,
                CompanyID = domainModel.CompanyID,
                UserID = domainModel.UserID,
            };
        }
    }
}
