using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Services.Facades;

namespace API.Controllers.Company
{
    [RoutePrefix("api/company-employee")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CompanyEmployeeApiController : ApiController
    {
        private readonly CompanyEmployeeFacade _companyEmployeeFacade;

        private const string DEFAULT_PHOTO_PATH = "~/Content/EmployeePhoto/Default/default.png";
        private const string PHOTO_FOLDER_PATH = "~/Content/EmployeePhoto";

        public CompanyEmployeeApiController(CompanyEmployeeFacade companyEmployeeFacade)
        {
            _companyEmployeeFacade = companyEmployeeFacade;
        }

        [HttpGet, Route("employees")]
        public async Task<IHttpActionResult> GetAll([FromUri] int companyId)
        {
            try
            {
                var employees = await _companyEmployeeFacade.EmployeeRepository.GetByCompanyIdAsync(companyId);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}