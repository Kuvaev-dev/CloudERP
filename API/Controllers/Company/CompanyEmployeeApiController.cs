using API.Models;
using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.Facades;

namespace API.Controllers.Company
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class CompanyEmployeeApiController : ControllerBase
    {
        private readonly CompanyEmployeeFacade _companyEmployeeFacade;
        private readonly IFileAdapterFactory _fileAdapterFactory;

        private const string DEFAULT_PHOTO_PATH = "~/EmployeePhoto/Default/default.png";

        public CompanyEmployeeApiController(
            CompanyEmployeeFacade companyEmployeeFacade,
            IFileAdapterFactory fileAdapterFactory)
        {
            _companyEmployeeFacade = companyEmployeeFacade;
            _fileAdapterFactory = fileAdapterFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAll(int companyId)
        {
            try
            {
                var employees = await _companyEmployeeFacade.EmployeeRepository.GetByCompanyIdAsync(companyId);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> EmployeeRegistration([FromBody] Employee employee)
        {
            if (employee == null) return BadRequest("Model cannot be null.");

            try
            {
                if (await _companyEmployeeFacade.EmployeeRepository.IsExists(employee))
                    return Conflict("An employee with the same TIN or email already exists.");

                await _companyEmployeeFacade.EmployeeRepository.AddAsync(employee);
                await SendRegistrationEmail(employee);

                return CreatedAtAction(nameof(GetById), new { id = employee.EmployeeID }, employee);
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
                var employee = await _companyEmployeeFacade.EmployeeRepository.GetByIdAsync(id);
                if (employee == null) return NotFound("Model not found.");
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        private async Task SendRegistrationEmail(Employee employee)
        {
            var subject = "Cloud ERP - Profile Registration";
            var body = $"<strong>Dear {employee.FullName},</strong><br/>" +
                       $"<p>Your profile has been successfully registered with Cloud ERP.</p>" +
                       $"<p>Username: {employee.Email}</p>" +
                       $"<p>Contact Number: {employee.ContactNumber}</p>" +
                       $"<p>Address: {employee.Address}</p>" +
                       $"<p>TIN: {employee.TIN}</p>" +
                       $"<p>Designation: {employee.Designation}</p>" +
                       $"<p>Description: {employee.Description}</p>" +
                       $"<p>Monthly Salary: {employee.MonthlySalary}</p>" +
                       $"<p>Registration Date: {employee.RegistrationDate}</p>";

            await _companyEmployeeFacade.EmailService.SendEmail(employee.Email, subject, body);
        }

        [HttpGet]
        public async Task<ActionResult<SalaryMV>> ProcessSalary(string TIN)
        {
            var employee = await _companyEmployeeFacade.EmployeeRepository.GetByTINAsync(TIN);
            if (employee == null) return NotFound($"Employee with TIN {TIN} not found.");

            return Ok(new SalaryMV
            {
                EmployeeID = employee.EmployeeID,
                EmployeeName = employee.FullName,
                TIN = employee.TIN,
                Designation = employee.Designation,
                TransferAmount = employee.MonthlySalary,
                SalaryMonth = DateTime.Now.ToString("MMMM"),
                SalaryYear = DateTime.Now.ToString("yyyy")
            });
        }

        [HttpPost]
        public async Task<ActionResult<string>> ConfirmSalary([FromBody] SalaryMV salaryMV)
        {
            try
            {
                var salary = new Salary
                {
                    EmployeeID = salaryMV.EmployeeID,
                    SalaryMonth = salaryMV.SalaryMonth,
                    SalaryYear = salaryMV.SalaryYear,
                    TransferAmount = salaryMV.TotalAmount <= 0 ? salaryMV.TransferAmount : salaryMV.TotalAmount
                };

                string message = await _companyEmployeeFacade.EmployeeSalaryService.ConfirmSalaryAsync(
                    salary,
                    salaryMV.UserID,
                    salaryMV.BranchID,
                    salaryMV.CompanyID);

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payroll>>> GetSalaryHistory(int branchId, int companyId)
        {
            try
            {
                var history = await _companyEmployeeFacade.PayrollRepository.GetSalaryHistoryAsync(branchId, companyId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<Payroll>> GetLatestSalaryInvoice()
        {
            try
            {
                var invoice = await _companyEmployeeFacade.PayrollRepository.GetLatestPayrollAsync();
                if (invoice == null) return BadRequest("Model not found.");
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}