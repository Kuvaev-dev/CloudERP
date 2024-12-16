using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;
using System;

namespace Domain.Mapping
{
    public class EmployeeMapper : IMapper<Employee, tblEmployee>
    {
        public Employee MapToDomain(tblEmployee dbModel)
        {
            return new Employee
            {
                EmployeeID = dbModel.EmployeeID,
                FullName = dbModel.Name,
                ContactNumber = dbModel.ContactNo,
                Address = dbModel.Address,
                Photo = dbModel.Photo,
                RegistrationDate = (DateTime)dbModel.RegistrationDate,
                CompanyID = dbModel.CompanyID,
                BranchID = dbModel.BranchID,
                UserID = dbModel.UserID
            };
        }

        public tblEmployee MapToDatabase(Employee domainModel)
        {
            return new tblEmployee
            {
                EmployeeID = domainModel.EmployeeID,
                Name = domainModel.FullName,
                ContactNo = domainModel.ContactNumber,
                Address = domainModel.Address,
                Photo = domainModel.Photo,
                IsFirstLogin = domainModel.IsFirstLogin,
                RegistrationDate = domainModel.RegistrationDate,
                CompanyID = domainModel.CompanyID,
                BranchID = domainModel.BranchID,
                UserID = domainModel.UserID
            };
        }
    }
}
