using CloudERP.Models;
using Domain.Models;

namespace CloudERP.Mapping
{
    public class UserTypeMapper
    {
        public static UserType MapToDomain(UserTypeMV viewModel)
        {
            return new UserType
            {
                UserTypeName = viewModel.UserTypeName
            };
        }

        public static UserTypeMV MapToViewModel(UserType domainModel)
        {
            return new UserTypeMV
            {
                UserTypeID = domainModel.UserTypeID,
                UserTypeName = domainModel.UserTypeName
            };
        }
    }
}