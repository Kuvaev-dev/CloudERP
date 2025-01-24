using CloudERP.Facades;
using CloudERP.Helpers;
using CloudERP.Models;
using Domain.Models;
using Domain.Models.FinancialModels;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class CompanyEmployeeController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly CompanyEmployeeFacade _companyEmployeeFacade;

        private const string DEFAULT_PHOTO_PATH = "~/Content/EmployeePhoto/Default/default.png";
        private const string PHOTO_FOLDER_PATH = "~/Content/EmployeePhoto";

        public CompanyEmployeeController(
            SessionHelper sessionHelper, 
            CompanyEmployeeFacade companyEmployeeFacade)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _companyEmployeeFacade = companyEmployeeFacade ?? throw new ArgumentNullException(nameof(CompanyEmployeeFacade));
        }

        // GET: Employees
        public async Task<ActionResult> Employees()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                return View(await _companyEmployeeFacade.EmployeeRepository.GetByCompanyIdAsync(_sessionHelper.CompanyID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> EmployeeRegistration()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                ViewBag.BranchID = new SelectList(await _companyEmployeeFacade.BranchRepository.GetByCompanyAsync(_sessionHelper.CompanyID), "BranchID", "BranchName", 0);
                return View(new Employee());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeRegistration(EmployeeMV employee)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                employee.Employee.CompanyID = _sessionHelper.CompanyID;
                employee.Employee.UserID = null;
                employee.Employee.RegistrationDate = DateTime.Now;
                employee.Employee.IsFirstLogin = true;

                if (ModelState.IsValid)
                {
                    await _companyEmployeeFacade.EmployeeRepository.AddAsync(employee.Employee);

                    string photoPath = null;

                    if (employee.LogoFile != null)
                    {
                        var fileName = $"{employee.Employee.CompanyID}.jpg";

                        var fileAdapter = _companyEmployeeFacade.FileAdapterFactory.Create(employee.LogoFile);
                        photoPath = _companyEmployeeFacade.FileService.UploadPhoto(fileAdapter, PHOTO_FOLDER_PATH, fileName);
                    }

                    employee.Employee.Photo = photoPath ?? _companyEmployeeFacade.FileService.SetDefaultPhotoPath(DEFAULT_PHOTO_PATH);

                    await _companyEmployeeFacade.EmployeeRepository.UpdateAsync(employee.Employee);

                    var subject = "Employee Registration Successful";
                    var body = $"<strong>Dear {employee.Employee.FullName},</strong><br/><br/>" +
                               $"Your registration is successful. Here are your details:<br/>" +
                               $"Name: {employee.Employee.FullName}<br/>" +
                               $"Email: {employee.Employee.Email}<br/>" +
                               $"Contact No: {employee.Employee.ContactNumber}<br/>" +
                               $"Designation: {employee.Employee.Designation}<br/><br/>" +
                               $"Best regards,<br/>Company Team";
                    _companyEmployeeFacade.EmailService.SendEmail(employee.Employee.Email, subject, body);

                    return RedirectToAction("Employees");
                }

                ViewBag.BranchID = new SelectList(await _companyEmployeeFacade.BranchRepository.GetByCompanyAsync(_sessionHelper.CompanyID), "BranchID", "BranchName", employee.Employee.BranchID);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }

            return View(employee);
        }

        public ActionResult EmployeeSalary()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var salary = new SalaryMV
            {
                SalaryMonth = DateTime.Now.AddMonths(-1).ToString("MMMM"),
                SalaryYear = DateTime.Now.AddMonths(-1).ToString("yyyy")
            };

            return View(salary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeSalary(SalaryMV salary)
        {
            Session["SalaryMessage"] = string.Empty;

            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                int companyID = _sessionHelper.CompanyID;
                var employee = await _companyEmployeeFacade.EmployeeRepository.GetByTINAsync(salary.TIN);

                if (employee != null)
                {
                    salary.EmployeeID = employee.EmployeeID;
                    salary.EmployeeName = employee.FullName;
                    salary.Designation = employee.Designation;
                    salary.TIN = employee.TIN;
                    salary.TransferAmount = employee.MonthlySalary;
                    Session["SalaryMessage"] = string.Empty;
                }
                else
                {
                    Session["SalaryMessage"] = Resources.Messages.RecordNotFound;
                }

                salary.SalaryMonth = DateTime.Now.AddMonths(-1).ToString("MMMM");
                salary.SalaryYear = DateTime.Now.AddMonths(-1).ToString("yyyy");

                return View(salary);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeSalaryConfirm(SalaryMV salaryMV)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

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
                    _sessionHelper.UserID,
                    _sessionHelper.BranchID,
                    _sessionHelper.CompanyID);

                if (message.Contains("Succeed"))
                {
                    Session["SalaryMessage"] = message;

                    int payrollNo = await _companyEmployeeFacade.EmployeeSalaryService.GetLatestPayrollNumberAsync();

                    return RedirectToAction("PrintSalaryInvoice", new { id = payrollNo });
                }

                Session["SalaryMessage"] = message;
                return RedirectToAction("EmployeeSalary");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> SalaryHistory()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                return View(await _companyEmployeeFacade.PayrollRepository.GetSalaryHistoryAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> PrintSalaryInvoice(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                return View(await _companyEmployeeFacade.PayrollRepository.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}