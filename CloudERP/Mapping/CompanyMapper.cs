using CloudERP.Models;
using Domain.Models;

namespace CloudERP.Mapping
{
    public static class CompanyMapper
    {
        public static Company MapToDomain(CompanyMV viewModel)
        {
            return new Company
            {
                CompanyID = viewModel.CompanyID,
                Name = viewModel.Name,
                Logo = viewModel.Logo,
                Description = viewModel.Description
            };
        }

        public static CompanyMV MapToViewModel(Company domainModel)
        {
            return new CompanyMV
            {
                CompanyID = domainModel.CompanyID,
                Name = domainModel.Name,
                Logo = domainModel.Logo,
                Description = domainModel.Description
            };
        }
    }
}