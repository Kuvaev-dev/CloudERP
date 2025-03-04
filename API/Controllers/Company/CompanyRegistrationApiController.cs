using API.Models;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Facades;
using Utils.Helpers;

namespace API.Controllers.Company
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class CompanyRegistrationApiController : ControllerBase
    {
        private readonly CompanyRegistrationFacade _companyRegistrationFacade;
        private readonly PasswordHelper _passwordHelper;

        private const string DEFAULT_COMPANY_LOGO_PATH = "~/CompanyLogo/erp-logo.png";
        private const string DEFAULT_EMPLOYEE_PHOTO_PATH = "~/EmployeePhoto/Default/default.png";
        private const int MAIN_BRANCH_ID = 1;
        private const int DEFAULT_USER_TYPE_ID = 2;

        public CompanyRegistrationApiController(
            CompanyRegistrationFacade companyRegistrationFacade,
            PasswordHelper passwordHelper)
        {
            _companyRegistrationFacade = companyRegistrationFacade ?? throw new ArgumentNullException(nameof(companyRegistrationFacade));
            _passwordHelper = passwordHelper ?? throw new ArgumentNullException(nameof(passwordHelper));
        }

        [HttpPost]
        public async Task<ActionResult> Register([FromBody] RegistrationMV model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest("Incorrect details.");
            }

            try
            {
                if (await _companyRegistrationFacade.UserRepository.GetByEmailAsync(model.EmployeeEmail) != null)
                    return Conflict("User with provided Email already exists.");

                if (await _companyRegistrationFacade.CompanyRepository.GetByNameAsync(model.CompanyName) != null)
                    return Conflict("Company with provided Name already exists.");

                var company = new Domain.Models.Company
                {
                    Name = model.CompanyName,
                    Logo = DEFAULT_COMPANY_LOGO_PATH,
                    Description = string.Empty
                };
                await _companyRegistrationFacade.CompanyRepository.AddAsync(company);

                var branch = new Domain.Models.Branch
                {
                    BranchAddress = model.BranchAddress,
                    BranchContact = model.BranchContact,
                    BranchName = model.BranchName,
                    BranchTypeID = MAIN_BRANCH_ID,
                    CompanyID = company.CompanyID
                };
                await _companyRegistrationFacade.BranchRepository.AddAsync(branch);

                string hashedPassword = _passwordHelper.HashPassword(model.EmployeeContactNo, out string salt);
                var user = new Domain.Models.User
                {
                    ContactNo = model.EmployeeContactNo,
                    Email = model.EmployeeEmail,
                    FullName = model.EmployeeName,
                    IsActive = true,
                    Password = hashedPassword,
                    Salt = salt,
                    UserName = model.UserName,
                    UserTypeID = DEFAULT_USER_TYPE_ID
                };
                await _companyRegistrationFacade.UserRepository.AddAsync(user);

                var employee = new Employee
                {
                    Address = model.EmployeeAddress,
                    BranchID = branch.BranchID,
                    TIN = model.EmployeeTIN,
                    CompanyID = company.CompanyID,
                    ContactNumber = model.EmployeeContactNo,
                    Designation = model.EmployeeDesignation,
                    Description = string.Empty,
                    Email = model.EmployeeEmail,
                    MonthlySalary = model.EmployeeMonthlySalary,
                    UserID = user.UserID,
                    FullName = model.EmployeeName,
                    IsFirstLogin = true,
                    Photo = DEFAULT_EMPLOYEE_PHOTO_PATH
                };
                await _companyRegistrationFacade.EmployeeRepository.AddAsync(employee);

                SendEmail(employee, company);

                return Ok("Registration succeeded");
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        private void SendEmail(Employee employee, Domain.Models.Company company)
        {
            var subject = $"Welcome to {company.Name}";
            var body = $"Hello, {employee.FullName},\n\n" +
                       "Your registration succeeded. Here is your data:\n" +
                       $"Full Name: {employee.FullName}\n" +
                       $"Email: {employee.Email}\n" +
                       $"Contact Number: {employee.ContactNumber}\n\n" +
                       $"Best regards,\n{company.Name}'s command.";

            _companyRegistrationFacade.EmailService.SendEmail(employee.Email, subject, body);
        }
    }
}