using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;

namespace Domain.Mapping
{
    public class UserTypeMapper : BaseMapper<UserType, tblUserType>
    {
        public override UserType MapToDomain(tblUserType dbModel)
        {
            return new UserType
            {
                UserTypeID = dbModel.UserTypeID,
                UserTypeName = dbModel.UserType
            };
        }

        public override tblUserType MapToDatabase(UserType domainModel)
        {
            return new tblUserType
            {
                UserTypeID = domainModel.UserTypeID,
                UserType = domainModel.UserTypeName
            };
        }
    }
}
