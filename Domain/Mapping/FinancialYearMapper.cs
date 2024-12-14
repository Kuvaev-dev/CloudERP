using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;

namespace Domain.Mapping
{
    public class FinancialYearMapper : BaseMapper<FinancialYear, tblFinancialYear>
    {
        public override FinancialYear MapToDomain(tblFinancialYear dbModel)
        {
            return new FinancialYear
            {
                FinancialYearID = dbModel.FinancialYearID,
                FinancialYearName = dbModel.FinancialYear,
                StartDate = dbModel.StartDate,
                EndDate = dbModel.EndDate,
                IsActive = dbModel.IsActive,
                UserID = dbModel.UserID
            };
        }

        public override tblFinancialYear MapToDatabase(FinancialYear domainModel)
        {
            return new tblFinancialYear
            {
                FinancialYearID = domainModel.FinancialYearID,
                FinancialYear = domainModel.FinancialYearName,
                StartDate = domainModel.StartDate,
                EndDate = domainModel.EndDate,
                IsActive = domainModel.IsActive,
                UserID = domainModel.UserID
            };
        }
    }
}
