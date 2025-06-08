using Domain.RepositoryAccess;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using API.Factories;
using Microsoft.AspNetCore.Authorization;
using Domain.ServiceAccess;

namespace API.Controllers.Branch
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class BranchEmployeeApiController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IFileService _fileService;
        private readonly IFileAdapterFactory _fileAdapterFactory;

        private const string DEFAULT_EMPLOYEE_AVATAR_PATH = "~/EmployeePhoto/Default/default.png";

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
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> EmployeeRegistration([FromBody] Employee model)
        {
            if (model == null) return BadRequest("Model cannot be null.");

            try
            {
                if (await _employeeRepository.IsExists(model))
                    return Conflict("An employee with the same name already exists.");

                model.Photo ??= DEFAULT_EMPLOYEE_AVATAR_PATH;
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

        [HttpPost]
        public async Task<ActionResult<Employee>> EmployeeUpdation([FromBody] Employee model)
        {
            if (model == null) return BadRequest("Model cannot be null.");

            try
            {
                if (await _employeeRepository.IsExists(model))
                    return Conflict("An employee with the same name already exists.");

                model.Photo ??= DEFAULT_EMPLOYEE_AVATAR_PATH;
                await _employeeRepository.UpdateAsync(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}