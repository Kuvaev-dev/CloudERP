using API.Factories;
using API.Models;
using Domain.Models;
using Domain.Models.FinancialModels;
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

        private const string DEFAULT_PHOTO_PATH = "~/Content/EmployeePhoto/Default/default.png";
        private const string PHOTO_FOLDER_PATH = "~/Content/EmployeePhoto";

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
            if (Request.HasFormContentType && string.IsNullOrEmpty(Request.Form["model"]))
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
                    model.Photo = await _companyEmployeeFacade.FileService.UploadPhotoAsync(fileAdapter, PHOTO_FOLDER_PATH, fileName);
                }
                else
                {
                    model.Photo = _companyEmployeeFacade.FileService.SetDefaultPhotoPath(DEFAULT_PHOTO_PATH);
                }

                await _companyEmployeeFacade.EmployeeRepository.AddAsync(model);

                SendEmail(model);

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
                var employee = await _companyEmployeeFacade.EmployeeRepository.GetByIdAsync(id);
                if (employee == null) return NotFound();
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        private void SendEmail(Employee employee)
        {
            var subject = "Employee Registration Successful";
            var body = $"<strong>Dear {employee.FullName},</strong><br/><br/>" +
                       $"Your registration is successful. Here are your details:<br/>" +
                       $"Name: {employee.FullName}<br/>" +
                       $"Email: {employee.Email}<br/>" +
                       $"Contact No: {employee.ContactNumber}<br/>" +
                       $"Designation: {employee.Designation}<br/><br/>" +
                       $"Best regards,<br/>Company Team";

            _companyEmployeeFacade.EmailService.SendEmail(employee.Email, subject, body);
        }

        [HttpPost]
        public async Task<ActionResult<SalaryMV>> ProcessSalary(string TIN)
        {
            var employee = await _companyEmployeeFacade.EmployeeRepository.GetByTINAsync(TIN);
            if (employee == null) return NotFound($"Employee with TIN {TIN} not found.");

            return Ok(new SalaryMV
            {
                EmployeeID = employee.EmployeeID,
                EmployeeName = employee.FullName,
                Designation = employee.Designation,
                TIN = employee.TIN,
                TransferAmount = employee.MonthlySalary
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
                    TransferAmount = salaryMV.TransferAmount
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
                if (history == null) return NotFound();
                return Ok(history);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<Payroll>> GetSalaryInvoice(int id)
        {
            try
            {
                var invoice = await _companyEmployeeFacade.PayrollRepository.GetByIdAsync(id);
                if (invoice == null) return BadRequest();
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}