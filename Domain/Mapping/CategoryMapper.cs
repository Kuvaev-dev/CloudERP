using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;

namespace Domain.Mapping
{
    public class CategoryMapper : BaseMapper<Category, tblCategory>
    {
        public override Category MapToDomain(tblCategory dbModel)
        {
            return new Category
            {
                CategoryID = dbModel.CategoryID,
                CategoryName = dbModel.CategoryName,
                BranchID = dbModel.BranchID,
                BranchName = dbModel.tblBranch.BranchName,
                CompanyID = dbModel.CompanyID,
                CompanyName = dbModel.tblCompany.Name,
                UserID = dbModel.UserID,
                UserName = dbModel.tblUser?.FullName
            };
        }

        public override tblCategory MapToDatabase(Category domainModel)
        {
            return new tblCategory
            {
                CategoryID = domainModel.CategoryID,
                CategoryName = domainModel.CategoryName,
                BranchID = domainModel.BranchID,
                CompanyID = domainModel.CompanyID,
                UserID = domainModel.UserID
            };
        }
    }
}
