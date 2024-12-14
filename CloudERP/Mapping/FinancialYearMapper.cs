using CloudERP.Models;
using Domain.Models;

namespace CloudERP.Mapping
{
    public static class FinancialYearMapper
    {
        public static FinancialYear MapToDomain(FinancialYearMV viewModel)
        {
            return new FinancialYear
            {
                FinancialYearID = viewModel.FinancialYearID,
                FinancialYearName = viewModel.FinancialYearName,
                StartDate = viewModel.StartDate,
                EndDate = viewModel.EndDate,
                IsActive = viewModel.IsActive
            };
        }

        public static FinancialYearMV MapToViewModel(FinancialYear domainModel)
        {
            return new FinancialYearMV
            {
                FinancialYearID = domainModel.FinancialYearID,
                FinancialYearName = domainModel.FinancialYearName,
                StartDate = domainModel.StartDate,
                EndDate = domainModel.EndDate,
                IsActive = domainModel.IsActive
            };
        }
    }
}