using Domain.RepositoryAccess;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IEmailService _emailService;

        public BranchEmployeeApiController(
            IEmployeeRepository employeeRepository,
            IFileService fileService,
            IFileAdapterFactory fileAdapterFactory,
            IEmailService emailService)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _fileAdapterFactory = fileAdapterFactory ?? throw new ArgumentNullException(nameof(fileAdapterFactory));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
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

                await _employeeRepository.AddAsync(model);
                await SendRegistrationEmail(model);
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

        [HttpPut]
        public async Task<ActionResult<Employee>> EmployeeUpdation([FromBody] Employee model)
        {
            if (model == null) return BadRequest("Model cannot be null.");

            try
            {
                if (await _employeeRepository.IsExists(model))
                    return Conflict("An employee with the same name already exists.");

                await _employeeRepository.UpdateAsync(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        public async Task SendRegistrationEmail(Employee data)
        {
            var subject = "Cloud ERP - Profile Registration";
            var body = $"<strong>Dear {data.FullName},</strong><br/>" +
                       $"<p>Your profile has been successfully registered with Cloud ERP.</p>" +
                       $"<p>Username: {data.Email}</p>" +
                       $"<p>Contact Number: {data.ContactNumber}</p>" +
                       $"<p>Address: {data.Address}</p>" +
                       $"<p>TIN: {data.TIN}</p>" +
                       $"<p>Designation: {data.Designation}</p>" +
                       $"<p>Description: {data.Description}</p>" +
                       $"<p>Monthly Salary: {data.MonthlySalary}</p>" +
                       $"<p>Registration Date: {data.RegistrationDate}</p>";

            await _emailService.SendEmail(data.Email, subject, body);
        }
    }
}