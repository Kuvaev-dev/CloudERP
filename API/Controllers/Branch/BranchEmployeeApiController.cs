using Domain.RepositoryAccess;
using Utils.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using API.Factories;

namespace API.Controllers.Branch
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BranchEmployeeApiController : ControllerBase
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> Employee(int companyId, int branchId)
        {
            try
            {
                var employees = await _employeeRepository.GetByBranchAsync(companyId, branchId);
                if (employees == null) return NotFound();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> EmployeeRegistration()
        {
            if (!Request.HasFormContentType && string.IsNullOrEmpty(Request.Form["model"]))
            {
                return BadRequest("Invalid data.");
            }

            Employee model;
            try
            {
                model = JsonConvert.DeserializeObject<Employee>(Request.Form["model"]);
            }
            catch
            {
                return BadRequest("Invalid employee data format.");
            }

            if (model == null) return BadRequest("Invalid data.");

            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    var fileName = $"{model.UserID}.jpg";

                    var fileAdapter = _fileAdapterFactory.Create(file);
                    model.Photo = await _fileService.UploadPhotoAsync(fileAdapter, EMPLOYEE_AVATAR_PATH, fileName);
                }
                else
                {
                    model.Photo = _fileService.SetDefaultPhotoPath(DEFAULT_EMPLOYEE_AVATAR_PATH);
                }

                await _employeeRepository.AddAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = model.EmployeeID }, model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<Employee>> GetById(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee == null) return NotFound();

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost, Route("updation")]
        public async Task<ActionResult<Employee>> EmployeeUpdation()
        {
            if (!Request.HasFormContentType && string.IsNullOrEmpty(Request.Form["model"]))
            {
                return BadRequest("Invalid data.");
            }

            Employee model;
            try
            {
                model = Newtonsoft.Json.JsonConvert.DeserializeObject<Employee>(Request.Form["model"]);
            }
            catch
            {
                return BadRequest("Invalid employee data format.");
            }

            if (model == null) return BadRequest("Invalid data.");

            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    var fileName = $"{model.UserID}.jpg";

                    var fileAdapter = _fileAdapterFactory.Create(file);
                    model.Photo = await _fileService.UploadPhotoAsync(fileAdapter, EMPLOYEE_AVATAR_PATH, fileName);
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
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}