using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using API.Models;
using Domain.Models;
using Domain.Models.FinancialModels;
using Services.Facades;

namespace API.Controllers
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

        [HttpGet, Route("employees/{companyId:int}")]
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

                    var fileAdapter = _companyEmployeeFacade.FileAdapterFactory.Create(file);
                    model.Photo = _companyEmployeeFacade.FileService.UploadPhoto(fileAdapter, PHOTO_FOLDER_PATH, fileName);
                }
                else
                {
                    model.Photo = _companyEmployeeFacade.FileService.SetDefaultPhotoPath(DEFAULT_PHOTO_PATH);
                }

                await _companyEmployeeFacade.EmployeeRepository.AddAsync(model);

                SendEmail(model);

                return Created($"api/company-employee/{model.EmployeeID}", model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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

        [HttpPost, Route("salary/process")]
        public async Task<IHttpActionResult> ProcessSalary([FromBody] SalaryMV salary)
        {
            try
            {
                var employee = await _companyEmployeeFacade.EmployeeRepository.GetByTINAsync(salary.TIN);
                if (employee != null)
                {
                    salary.EmployeeID = employee.EmployeeID;
                    salary.EmployeeName = employee.FullName;
                    salary.Designation = employee.Designation;
                    salary.TIN = employee.TIN;
                    salary.TransferAmount = employee.MonthlySalary;
                }
                return Ok(salary);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("salary/confirm")]
        public async Task<IHttpActionResult> ConfirmSalary([FromBody] SalaryMV salaryMV)
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
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("salary/history")]
        public async Task<IHttpActionResult> GetSalaryHistory(int branchId, int companyId)
        {
            try
            {
                var history = await _companyEmployeeFacade.PayrollRepository.GetSalaryHistoryAsync(branchId, companyId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("salary/invoice/{id}")]
        public async Task<IHttpActionResult> GetSalaryInvoice(int id)
        {
            try
            {
                var invoice = await _companyEmployeeFacade.PayrollRepository.GetByIdAsync(id);
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}