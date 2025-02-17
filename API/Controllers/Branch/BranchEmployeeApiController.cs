using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.RepositoryAccess;
using Utils.Interfaces;
using DatabaseAccess.Factories;
using Domain.Models;
using System.Web;

namespace API.Controllers
{
    [RoutePrefix("api/branch-employee")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BranchEmployeeApiController : ApiController
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IFileService _fileService;
        private readonly IFileAdapterFactory _fileAdapterFactory;

        private const string EMPLOYEE_AVATAR_PATH = "~/Content/EmployeePhoto";
        private const string DEFAULT_EMPLOYEE_AVATAR_PATH = "~/Content/EmployeePhoto/Default/default.png";

        public BranchEmployeeApiController(
            IEmployeeRepository employeeRepository,
            IFileService fileService,
            IFileAdapterFactory fileAdapterFactory)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(IEmployeeRepository));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(IFileService));
            _fileAdapterFactory = fileAdapterFactory ?? throw new ArgumentNullException(nameof(IFileAdapterFactory));
        }

        [HttpGet, Route("employee/{companyId:int}/{branchId:int}")]
        public async Task<IHttpActionResult> Employee(int companyId, int branchId)
        {
            try
            {
                var employees = await _employeeRepository.GetByBranchAsync(companyId, branchId);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("registration")]
        public async Task<IHttpActionResult> EmployeeRegistration()
        {
            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count == 0 && string.IsNullOrEmpty(httpRequest.Form["model"]))
            {
                return BadRequest("Invalid data.");
            }

            Employee model;
            try
            {
                model = Newtonsoft.Json.JsonConvert.DeserializeObject<Employee>(httpRequest.Form["model"]);
            }
            catch
            {
                return BadRequest("Invalid employee data format.");
            }

            if (model == null) return BadRequest("Invalid data.");

            try
            {
                if (httpRequest.Files.Count > 0)
                {
                    var file = httpRequest.Files[0];
                    var fileName = $"{model.UserID}.jpg";

                    var fileAdapter = _fileAdapterFactory.Create(file);
                    model.Photo = _fileService.UploadPhoto(fileAdapter, EMPLOYEE_AVATAR_PATH, fileName);
                }
                else
                {
                    model.Photo = _fileService.SetDefaultPhotoPath(DEFAULT_EMPLOYEE_AVATAR_PATH);
                }

                await _employeeRepository.AddAsync(model);
                return Created($"api/branch-employee/{model.EmployeeID}", model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee == null) return NotFound();

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("updation")]
        public async Task<IHttpActionResult> EmployeeUpdation()
        {
            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count == 0 && string.IsNullOrEmpty(httpRequest.Form["model"]))
            {
                return BadRequest("Invalid data.");
            }

            Employee model;
            try
            {
                model = Newtonsoft.Json.JsonConvert.DeserializeObject<Employee>(httpRequest.Form["model"]);
            }
            catch
            {
                return BadRequest("Invalid employee data format.");
            }

            if (model == null) return BadRequest("Invalid data.");

            try
            {
                if (httpRequest.Files.Count > 0)
                {
                    var file = httpRequest.Files[0];
                    var fileName = $"{model.UserID}.jpg";

                    var fileAdapter = _fileAdapterFactory.Create(file);
                    model.Photo = _fileService.UploadPhoto(fileAdapter, EMPLOYEE_AVATAR_PATH, fileName);
                }
                else
                {
                    model.Photo = _fileService.SetDefaultPhotoPath(DEFAULT_EMPLOYEE_AVATAR_PATH);
                }

                await _employeeRepository.UpdateAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}