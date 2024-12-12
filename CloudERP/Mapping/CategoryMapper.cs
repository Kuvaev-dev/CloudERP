using CloudERP.Models;
using Domain.Models;

namespace CloudERP.Mapping
{
    public class CategoryMapper
    {
        public static Category MapToDomain(CategoryMV viewModel)
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

        public static CategoryMV MapToViewModel(Category domainModel)
        {
            return new CategoryMV
            {
                CategoryID = domainModel.CategoryID,
                CategoryName = domainModel.CategoryName
            };
        }
    }
}