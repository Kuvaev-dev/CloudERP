using CloudERP.Models;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace CloudERP.Mapping
{
    public static class CustomerMapper
    {
        public static Customer MapToDomain(CustomerMV viewModel)
        {
            return new Customer
            {
                CustomerID = viewModel.CustomerID,
                Customername = viewModel.CustomerName,
                CustomerContact = viewModel.CustomerContact,
                CustomerAddress = viewModel.CustomerAddress,
                Description = viewModel.Description,
                CompanyID = viewModel.CompanyID,
                BranchID = viewModel.BranchID,
                UserID = viewModel.UserID
            };
        }

        public static CustomerMV MapToViewModel(Customer domainModel)
        {
            return new CustomerMV
            {
                CustomerID = domainModel.CustomerID,
                CustomerName = domainModel.Customername,
                CustomerContact = domainModel.CustomerContact,
                CustomerAddress = domainModel.CustomerAddress,
                Description = domainModel.Description,
                CompanyID = domainModel.CompanyID,
                BranchID = domainModel.BranchID,
                UserID = domainModel.UserID
            };
        }

        public static BranchsCustomersMV MapToBranchsCustomersMV(Customer domainModel)
        {
            return new BranchsCustomersMV
            {
                CompanyName = domainModel.CompanyName,
                BranchName = domainModel.BranchName,
                Customername = domainModel.Customername,
                CustomerContact = domainModel.CustomerContact,
                CustomerAddress = domainModel.CustomerAddress,
                Description = domainModel.Description,
                User = domainModel.UserName
            };
        }

        public static IEnumerable<BranchsCustomersMV> MapToBranchsCustomersMV(IEnumerable<Customer> domainModels)
        {
            return domainModels.Select(MapToBranchsCustomersMV);
        }
    }
}