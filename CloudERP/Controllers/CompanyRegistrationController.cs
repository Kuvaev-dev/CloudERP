using CloudERP.Helpers;
using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;
using Domain.Services;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class CompanyRegistrationController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly IBranchService _branchService;
        private readonly IUserService _userService;
        private readonly IEmployeeService _employeeService;
        private readonly IMapper<Company, CompanyMV> _companyMapper;
        private readonly IMapper<Branch, BranchMV> _branchMapper;
        private readonly IMapper<User, UserMV> _userMapper;
        private readonly IMapper<Employee, EmployeeMV> _employeeMapper;
        private readonly EmailService _emailService;
        private readonly SessionHelper _sessionHelper;
        private readonly PasswordHelper _passwordHelper;

        public CompanyRegistrationController(
            ICompanyService companyService,
            IBranchService branchService,
            IUserService userService,
            IEmployeeService employeeService,
            EmailService emailService,
            SessionHelper sessionHelper,
            PasswordHelper passwordHelper,
            IMapper<Company, CompanyMV> companyMapper,
            IMapper<Branch, BranchMV> branchMapper,
            IMapper<User, UserMV> userMapper,
            IMapper<Employee, EmployeeMV> employeeMapper)
        {
            _companyService = companyService;
            _branchService = branchService;
            _userService = userService;
            _employeeService = employeeService;
            _emailService = emailService;
            _sessionHelper = sessionHelper;
            _passwordHelper = passwordHelper;
            _companyMapper = companyMapper;
            _branchMapper = branchMapper;
            _userMapper = userMapper;
            _employeeMapper = employeeMapper;
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
                // Перевірка існування користувача
                if (await _userService.GetByEmailAsync(model.EmployeeEmail) != null)
                {
                    ModelState.AddModelError("", Resources.Messages.UsernameAlreadyExists);
                    return View(model);
                }

                // Перевірка існування компанії
                if (await _companyService.CheckCompanyExistsAsync(model.CompanyName))
                {
                    ModelState.AddModelError("", Resources.Messages.AlreadyExists);
                    return View(model);
                }

                // Створення компанії
                var company = _companyMapper.MapToDomain(new CompanyMV
                {
                    Name = model.CompanyName,
                    Logo = "~/Content/CompanyLogo/erp-logo.png",
                    Description = string.Empty
                });
                await _companyService.CreateAsync(company);

                // Створення відділення
                var branch = _branchMapper.MapToDomain(new BranchMV
                {
                    BranchAddress = model.BranchAddress,
                    BranchContact = model.BranchContact,
                    BranchName = model.BranchName,
                    BranchTypeID = 1,
                    CompanyID = company.CompanyID
                });
                await _branchService.CreateAsync(branch);

                // Створення користувача
                string hashedPassword = _passwordHelper.HashPassword(model.EmployeeContactNo, out string salt);
                var user = _userMapper.MapToDomain(new UserMV
                {
                    ContactNo = model.EmployeeContactNo,
                    Email = model.EmployeeEmail,
                    FullName = model.EmployeeName,
                    IsActive = true,
                    Password = hashedPassword,
                    Salt = salt,
                    UserName = model.UserName,
                    UserTypeID = 2
                });
                await _userService.CreateAsync(user);

                // Створення співробітника
                var employee = _employeeMapper.MapToDomain(new EmployeeMV
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
                });
                await _employeeService.CreateAsync(employee);

                // Відправлення листа
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
