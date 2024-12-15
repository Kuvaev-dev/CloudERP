using CloudERP.Models;
using System.Collections.Generic;
using System.Linq;

namespace CloudERP.Mapping
{
    public static class UserMapper
    {
        public static UserMV MapToViewModel(Domain.Models.User domainModel)
        {
            return new UserMV
            {
                UserID = domainModel.UserID,
                FullName = domainModel.FullName,
                Email = domainModel.Email,
                ContactNo = domainModel.ContactNo,
                UserName = domainModel.UserName,
                IsActive = domainModel.IsActive,
                UserTypeID = domainModel.UserTypeID,
                UserTypeName = domainModel.UserTypeName,
                BranchName = domainModel.BranchName
            };
        }

        public static Domain.Models.User MapToDomain(UserMV viewModel)
        {
            return new Domain.Models.User
            {
                UserID = viewModel.UserID,
                FullName = viewModel.FullName,
                Email = viewModel.Email,
                ContactNo = viewModel.ContactNo,
                UserName = viewModel.UserName,
                IsActive = viewModel.IsActive,
                UserTypeID = viewModel.UserTypeID
            };
        }

        public static IEnumerable<UserMV> MapToViewModel(IEnumerable<Domain.Models.User> domainModels)
        {
            return domainModels.Select(MapToViewModel);
        }

        public static IEnumerable<Domain.Models.User> MapToDomain(IEnumerable<UserMV> viewModels)
        {
            return viewModels.Select(MapToDomain);
        }
    }
}