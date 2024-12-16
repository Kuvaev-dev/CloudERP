using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;

namespace Domain.Mapping
{
    public class CompanyMapper : IMapper<Company, tblCompany>
    {
        public Company MapToDomain(tblCompany dbModel)
        {
            return new Company
            {
                CompanyID = dbModel.CompanyID,
                Name = dbModel.Name,
                Logo = dbModel.Logo,
                Description = dbModel.Description
            };
        }

        public tblCompany MapToDatabase(Company domainModel)
        {
            return new tblCompany
            {
                CompanyID = domainModel.CompanyID,
                Name = domainModel.Name,
                Logo = domainModel.Logo,
                Description = domainModel.Description
            };
        }
    }
}
