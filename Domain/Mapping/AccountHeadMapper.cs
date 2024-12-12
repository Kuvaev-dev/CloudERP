using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;

namespace Domain.Mapping
{
    public class AccountHeadMapper : BaseMapper<AccountHead, tblAccountHead>
    {
        public override AccountHead MapToDomain(tblAccountHead dbModel)
        {
            return new AccountHead
            {
                AccountHeadID = dbModel.AccountHeadID,
                AccountHeadName = dbModel.AccountHeadName,
                Code = dbModel.Code,
                UserID = dbModel.UserID,
                FullName = dbModel.tblUser != null ? dbModel.tblUser.FullName : string.Empty
            };
        }

        public override tblAccountHead MapToDatabase(AccountHead domainModel)
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
