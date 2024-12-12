using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;

namespace CloudERP.Mapping
{
    public class CategoryMapper : BaseMapper<Category, CategoryMV>
    {
        public override Category MapToDomain(CategoryMV viewModel)
        {
            return new Category
            {
                CategoryID = viewModel.CategoryID,
                CategoryName = viewModel.CategoryName,
                BranchID = viewModel.BranchID,
                CompanyID = viewModel.CompanyID,
                UserID = viewModel.UserID
            };
        }

        public override CategoryMV MapToViewModel(Category domainModel)
        {
            return new CategoryMV
            {
                CategoryID = domainModel.CategoryID,
                CategoryName = domainModel.CategoryName
            };
        }
    }
}