using DatabaseAccess;
using Mapping.Models;

namespace Mapping
{
    public static class AccountControlMapper
    {
        public static AccountControl MapToDomain(tblAccountControl dbModel)
        {
            return new AccountControl
            {
                AccountControlID = dbModel.AccountControlID,
                AccountControlName = dbModel.AccountControlName,
                AccountHeadID = dbModel.AccountHeadID,
                BranchID = dbModel.BranchID,
                CompanyID = dbModel.CompanyID,
                UserID = dbModel.UserID,
            };
        }

        public static tblAccountControl MapToDatabase(AccountControl domainModel)
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
