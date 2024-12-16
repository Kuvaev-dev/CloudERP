using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;

namespace CloudERP.Mapping
{
    public class BranchMapper : BaseMapper<Branch, BranchMV>
    {
        public override BranchMV MapToViewModel(Branch domainModel)
        {
            return new BranchMV
            {
                BranchID = domainModel.BranchID,
                BranchName = domainModel.BranchName,
                BranchContact = domainModel.BranchContact,
                BranchAddress = domainModel.BranchAddress,
                CompanyID = domainModel.CompanyID,
                ParentBranchID = domainModel.ParentBranchID,
                BranchTypeID = domainModel.BranchTypeID,
                BranchTypeName = domainModel.BranchTypeName
            };
        }

        public override Branch MapToDomain(BranchMV viewModel)
        {
            return new Branch
            {
                BranchID = viewModel.BranchID,
                BranchName = viewModel.BranchName,
                BranchContact = viewModel.BranchContact,
                BranchAddress = viewModel.BranchAddress,
                CompanyID = viewModel.CompanyID,
                ParentBranchID = viewModel.ParentBranchID,
                BranchTypeID = viewModel.BranchTypeID
            };
        }
    }
}