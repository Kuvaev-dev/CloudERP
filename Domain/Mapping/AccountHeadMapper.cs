using DatabaseAccess;
using Domain.Models;

namespace Domain.Mapping
{
    public static class AccountHeadMapper
    {
        public static AccountHead MapToDomain(tblAccountHead dbModel)
        {
            return new AccountHead
            {
                AccountHeadID = dbModel.AccountHeadID,
                AccountHeadName = dbModel.AccountHeadName,
                Code = dbModel.Code,
                UserID = dbModel.UserID
            };
        }

        public static tblAccountHead MapToDatabase(AccountHead domainModel)
        {
            return new tblAccountHead
            {
                AccountHeadID = domainModel.AccountHeadID,
                AccountHeadName = domainModel.AccountHeadName,
                Code = domainModel.Code,
                UserID = domainModel.UserID
            };
        }
    }
}
