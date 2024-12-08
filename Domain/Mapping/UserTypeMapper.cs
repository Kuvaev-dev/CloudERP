using DatabaseAccess;
using Domain.Models;

namespace Domain.Mapping
{
    public static class UserTypeMapper
    {
        public static UserType MapToDomain(tblUserType dbModel)
        {
            return new UserType
            {
                UserTypeID = dbModel.UserTypeID,
                UserTypeName = dbModel.UserType
            };
        }

        public static tblUserType MapToDatabase(UserType domainModel)
        {
            return new tblUserType
            {
                UserTypeID = domainModel.UserTypeID,
                UserType = domainModel.UserTypeName
            };
        }
    }
}
