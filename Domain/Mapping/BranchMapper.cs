using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;

namespace Domain.Mapping
{
    public class BranchMapper : IMapper<Branch, tblBranch>
    {
        public Branch MapToDomain(tblBranch dbModel)
        {
            return new Branch
            {
                BranchID = dbModel.BranchID,
                BranchName = dbModel.BranchName,
                BranchContact = dbModel.BranchContact,
                BranchAddress = dbModel.BranchAddress,
                CompanyID = dbModel.CompanyID,
                ParentBranchID = dbModel.BrchID,
                BranchTypeID = dbModel.BranchTypeID,
                BranchTypeName = dbModel.tblBranchType?.BranchType
            };
        }

        public tblBranch MapToDatabase(Branch domainModel)
        {
            return new tblBranch
            {
                BranchID = domainModel.BranchID,
                BranchName = domainModel.BranchName,
                BranchContact = domainModel.BranchContact,
                BranchAddress = domainModel.BranchAddress,
                CompanyID = domainModel.CompanyID,
                BrchID = domainModel.ParentBranchID,
                BranchTypeID = domainModel.BranchTypeID
            };
        }
    }
}
