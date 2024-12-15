using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;

namespace Domain.Mapping
{
    public class UserMapper : IMapper<User, tblUser>
    {
        public User MapToDomain(tblUser dbModel)
        {
            return new User
            {
                UserID = dbModel.UserID,
                FullName = dbModel.FullName,
                Email = dbModel.Email,
                ContactNo = dbModel.ContactNo,
                UserName = dbModel.UserName,
                Password = dbModel.Password,
                Salt = dbModel.Salt,
                UserTypeID = dbModel.UserTypeID,
                IsActive = dbModel.IsActive,
                UserTypeName = dbModel.tblUserType?.UserType,
                BranchName = "Unknown (Temporary)"
            };
        }

        public tblUser MapToDatabase(User domainModel)
        {
            return new tblUser
            {
                UserID = domainModel.UserID,
                FullName = domainModel.FullName,
                Email = domainModel.Email,
                ContactNo = domainModel.ContactNo,
                UserName = domainModel.UserName,
                Password = domainModel.Password,
                Salt = domainModel.Salt,
                UserTypeID = domainModel.UserTypeID,
                IsActive = domainModel.IsActive
            };
        }
    }
}
