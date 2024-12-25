using CloudERP.Helpers;
using CloudERP.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class CompanyRegistrationController : Controller
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly EmailService _emailService;
        private readonly SessionHelper _sessionHelper;
        private readonly PasswordHelper _passwordHelper;

        public CompanyRegistrationController(
            ICompanyRepository companyRepository,
            IBranchRepository branchRepository,
            IUserRepository userRepository,
            IEmployeeRepository employeeRepository,
            EmailService emailService,
            SessionHelper sessionHelper,
            PasswordHelper passwordHelper)
        {
            _companyRepository = companyRepository;
            _branchRepository = branchRepository;
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _emailService = emailService;
            _sessionHelper = sessionHelper;
            _passwordHelper = passwordHelper;
        }

        // GET: CompanyRegistration/RegistrationForm
        public ActionResult RegistrationForm()
        {
            if (!_sessionHelper.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            return View(new RegistrationMV());
        }

        // POST: CompanyRegistration/RegistrationForm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegistrationForm(RegistrationMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Message = Resources.Messages.PleaseProvideCorrectDetails;
                return View(model);
            }

            try
            {
                if (await _userRepository.GetByEmailAsync(model.EmployeeEmail) != null)
                {
                    ModelState.AddModelError("", Resources.Messages.UsernameAlreadyExists);
                    return View(model);
                }

                if (await _companyRepository.GetByNameAsync(model.CompanyName) != null)
                {
                    ModelState.AddModelError("", Resources.Messages.AlreadyExists);
                    return View(model);
                }

                var company = new Company
                {
                    Name = model.CompanyName,
                    Logo = "~/Content/CompanyLogo/erp-logo.png",
                    Description = string.Empty
                };
                await _companyRepository.AddAsync(company);

                var branch = new Branch
                {
                    BranchAddress = model.BranchAddress,
                    BranchContact = model.BranchContact,
                    BranchName = model.BranchName,
                    BranchTypeID = 1,
                    CompanyID = company.CompanyID
                };
                await _branchRepository.AddAsync(branch);

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
                    UserTypeID = 2
                };
                await _userRepository.AddAsync(user);

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
                    Photo = "~/Content/EmployeePhoto/Default/default.png"
                };
                await _employeeRepository.AddAsync(employee);

                var subject = "Welcome to the Company";
                var body = $"Hello {employee.FullName},\n\n" +
                           $"Your registration is successful. Here are your details:\n" +
                           $"Name: {employee.FullName}\n" +
                           $"Email: {employee.Email}\n" +
                           $"Contact No: {employee.ContactNumber}\n\n" +
                           $"Best regards,\nCompany Team";
                _emailService.SendEmail(employee.Email, subject, body);

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
