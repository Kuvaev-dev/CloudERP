using CloudERP.Models;
using Domain.Models;

namespace CloudERP.Mapping
{
    public static class EmployeeMapper
    {
        public static EmployeeMV MapToViewModel(Employee domainModel)
        {
            return new EmployeeMV
            {
                EmployeeID = domainModel.EmployeeID,
                FullName = domainModel.FullName,
                ContactNumber = domainModel.ContactNumber,
                Address = domainModel.Address,
                Photo = domainModel.Photo,
                IsFirstLogin = domainModel.IsFirstLogin,
                RegistrationDate = domainModel.RegistrationDate,
                CompanyID = domainModel.CompanyID,
                BranchID = domainModel.BranchID,
                UserID = domainModel.UserID
            };
        }

        public static Employee MapToDomain(EmployeeMV viewModel)
        {
            return new Employee
            {
                EmployeeID = viewModel.EmployeeID,
                FullName = viewModel.FullName,
                ContactNumber = viewModel.ContactNumber,
                Address = viewModel.Address,
                Photo = viewModel.Photo,
                IsFirstLogin = viewModel.IsFirstLogin,
                RegistrationDate = viewModel.RegistrationDate,
                CompanyID = viewModel.CompanyID,
                BranchID = viewModel.BranchID,
                UserID = viewModel.UserID
            };
        }
    }
}