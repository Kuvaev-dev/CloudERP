using CloudERP.Facades;
using CloudERP.Helpers;
using CloudERP.Models;
using Domain.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class CompanyRegistrationController : Controller
    {
        private readonly CompanyRegistrationFacade _companyRegistrationFacade;
        private readonly SessionHelper _sessionHelper;
        private readonly PasswordHelper _passwordHelper;

        private const string DEFAULT_COMPANY_LOGO_PATH = "~/Content/CompanyLogo/erp-logo.png";
        private const string DEFAULT_EMPLOYEE_PHOTO_PATH = "~/Content/EmployeePhoto/Default/default.png";
        private const int MAIN_BRANCH_ID = 1;
        private const int DEFAULT_USER_TYPE_ID = 2;

        public CompanyRegistrationController(
            CompanyRegistrationFacade companyRegistrationFacade,
            SessionHelper sessionHelper,
            PasswordHelper passwordHelper)
        {
            _companyRegistrationFacade = companyRegistrationFacade ?? throw new ArgumentNullException(nameof(CompanyRegistrationFacade));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _passwordHelper = passwordHelper ?? throw new ArgumentNullException(nameof(PasswordHelper));
        }

        // GET: CompanyRegistration/RegistrationForm
        public ActionResult RegistrationForm()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new RegistrationMV());
        }

        // POST: CompanyRegistration/RegistrationForm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegistrationForm(RegistrationMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.Message = Resources.Messages.PleaseProvideCorrectDetails;
                return View(model);
            }

            try
            {
                if (await _companyRegistrationFacade.UserRepository.GetByEmailAsync(model.EmployeeEmail) != null)
                {
                    ModelState.AddModelError("", Resources.Messages.AlreadyExists);
                    return View(model);
                }

                if (await _companyRegistrationFacade.CompanyRepository.GetByNameAsync(model.CompanyName) != null)
                {
                    ModelState.AddModelError("", Resources.Messages.AlreadyExists);
                    return View(model);
                }

                var company = new Company
                {
                    Name = model.CompanyName,
                    Logo = DEFAULT_COMPANY_LOGO_PATH,
                    Description = string.Empty
                };
                await _companyRegistrationFacade.CompanyRepository.AddAsync(company);

                var branch = new Branch
                {
                    BranchAddress = model.BranchAddress,
                    BranchContact = model.BranchContact,
                    BranchName = model.BranchName,
                    BranchTypeID = MAIN_BRANCH_ID,
                    CompanyID = company.CompanyID
                };
                await _companyRegistrationFacade.BranchRepository.AddAsync(branch);

                string hashedPassword = _passwordHelper.HashPassword(model.EmployeeContactNo, out string salt);
                var user = new User
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
                    Email = model.EmployeeEmail,
                    MonthlySalary = model.EmployeeMonthlySalary,
                    UserID = user.UserID,
                    FullName = model.EmployeeName,
                    IsFirstLogin = true,
                    Photo = DEFAULT_EMPLOYEE_PHOTO_PATH
                };
                await _companyRegistrationFacade.EmployeeRepository.AddAsync(employee);

                var subject = "Welcome to the Company";
                var body = $"Hello {employee.FullName},\n\n" +
                           $"Your registration is successful. Here are your details:\n" +
                           $"Name: {employee.FullName}\n" +
                           $"Email: {employee.Email}\n" +
                           $"Contact No: {employee.ContactNumber}\n\n" +
                           $"Best regards,\nCompany Team";
                _companyRegistrationFacade.EmailService.SendEmail(employee.Email, subject, body);

                ViewBag.Message = Resources.Messages.RegistrationSuccessful;
                return RedirectToAction("Login", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}
