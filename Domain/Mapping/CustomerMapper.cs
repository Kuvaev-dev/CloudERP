using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;

namespace Domain.Mapping
{
    public class CustomerMapper : BaseMapper<Customer, tblCustomer>
    {
        public override Customer MapToDomain(tblCustomer dbModel)
        {
            return new Customer
            {
                CustomerID = dbModel.CustomerID,
                Customername = dbModel.Customername,
                CustomerContact = dbModel.CustomerContact,
                CustomerAddress = dbModel.CustomerAddress,
                Description = dbModel.Description,
                BranchID = dbModel.BranchID,
                CompanyID = dbModel.CompanyID,
                UserID = dbModel.UserID,
                BranchName = dbModel.tblBranch?.BranchName,
                CompanyName = dbModel.tblCompany?.Name,
                UserName = dbModel.tblUser?.UserName
            };
        }

        public override tblCustomer MapToDatabase(Customer domainModel)
        {
            return new tblCustomer
            {
                CustomerID = domainModel.CustomerID,
                Customername = domainModel.Customername,
                CustomerContact = domainModel.CustomerContact,
                CustomerAddress = domainModel.CustomerAddress,
                Description = domainModel.Description,
                BranchID = domainModel.BranchID,
                CompanyID = domainModel.CompanyID,
                UserID = domainModel.UserID
            };
        }
    }
}
