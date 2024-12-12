using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;

namespace CloudERP.Mapping
{
    public class UserTypeMapper : BaseMapper<UserType, UserTypeMV>
    {
        public override UserType MapToDomain(UserTypeMV viewModel)
        {
            return new UserType
            {
                UserTypeName = viewModel.UserTypeName
            };
        }

        public override UserTypeMV MapToViewModel(UserType domainModel)
        {
            return new UserTypeMV
            {
                UserTypeID = domainModel.UserTypeID,
                UserTypeName = domainModel.UserTypeName
            };
        }
    }
}