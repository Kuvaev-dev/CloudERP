using DatabaseAccess;
using Domain.Models;

namespace Domain.Mapping
{
    public static class AccountSubControlMapper
    {
        public static AccountSubControl MapToDomain(tblAccountSubControl dbModel)
        {
            return new AccountSubControl
            {
                AccountSubControlID = dbModel.AccountSubControlID,
                AccountSubControlName = dbModel.AccountSubControlName,
                AccountControlID = dbModel.AccountControlID,
                AccountControlName = dbModel.tblAccountControl?.AccountControlName ?? "Unknown",
                AccountHeadID = dbModel.AccountHeadID,
                AccountHeadName = dbModel.tblAccountHead?.AccountHeadName ?? "Unknown",
                CompanyID = dbModel.CompanyID,
                BranchID = dbModel.BranchID,
                UserID = dbModel.UserID,
                UserName = dbModel.tblUser?.FullName ?? "Unknown"
            };
        }

        public static tblAccountSubControl MapToDatabase(AccountSubControl domainModel)
        {
            return new tblAccountSubControl
            {
                AccountSubControlID = domainModel.AccountSubControlID,
                AccountSubControlName = domainModel.AccountSubControlName,
                AccountControlID = domainModel.AccountControlID,
                AccountHeadID = domainModel.AccountHeadID,
                CompanyID = domainModel.CompanyID,
                BranchID = domainModel.BranchID,
                UserID = domainModel.UserID
            };
        }
    }
}
