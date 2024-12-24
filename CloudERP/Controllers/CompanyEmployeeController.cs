using CloudERP.Helpers;
using CloudERP.Models;
using DatabaseAccess;
using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class CompanyEmployeeController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SalaryTransaction _salaryTransaction;
        private readonly SessionHelper _sessionHelper;
        private readonly EmailService _emailService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBranchRepository _branchRepository;

        public CompanyEmployeeController(CloudDBEntities db, SalaryTransaction salaryTransaction, IEmployeeRepository employeeRepository, SessionHelper sessionHelper, IBranchRepository branchRepository, EmailService emailService)
        {
            _db = db;
            _salaryTransaction = salaryTransaction;
            _employeeRepository = employeeRepository;
            _sessionHelper = sessionHelper;
            _branchRepository = branchRepository;
            _emailService = emailService;
        }

        // GET: Employees
        public async Task<ActionResult> Employees()
        {
            if (!_sessionHelper.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                return View(await _employeeRepository.GetByCompanyIdAsync(_sessionHelper.CompanyID));
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
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                ViewBag.BranchID = new SelectList(await _branchRepository.GetByCompanyAsync(_sessionHelper.CompanyID), "BranchID", "BranchName", 0);
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
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                employee.Employee.CompanyID = _sessionHelper.CompanyID;
                employee.Employee.UserID = null;
                employee.Employee.RegistrationDate = DateTime.Now;
                employee.Employee.IsFirstLogin = true;

                if (ModelState.IsValid)
                {
                    await _employeeRepository.AddAsync(employee.Employee);

                    if (employee.LogoFile != null)
                    {
                        var folder = "~/Content/EmployeePhoto";
                        var file = $"{employee.Employee.CompanyID}.jpg";

                        var response = FileHelper.UploadPhoto(employee.LogoFile, folder, file);
                        if (response)
                        {
                            var filePath = Server.MapPath($"{folder}/{file}");
                            if (System.IO.File.Exists(filePath))
                            {
                                employee.Employee.Photo = $"{folder}/{file}";
                            }
                            else
                            { 
                                employee.Employee.Photo = "~/Content/EmployeePhoto/Default/default.png";
                            }
                            await _employeeRepository.UpdateAsync(employee.Employee);
                        }
                        else
                        {
                            employee.Employee.Photo = "~/Content/EmployeePhoto/Default/default.png";
                            await _employeeRepository.UpdateAsync(employee.Employee);
                        }
                    }
                    else
                    {
                        employee.Employee.Photo = "~/Content/EmployeePhoto/Default/default.png";
                        await _employeeRepository.UpdateAsync(employee.Employee);
                    }

                    // Send email
                    var subject = "Employee Registration Successful";
                    var body = $"<strong>Dear {employee.Employee.FullName},</strong><br/><br/>" +
                               $"Your registration is successful. Here are your details:<br/>" +
                               $"Name: {employee.Employee.FullName}<br/>" +
                               $"Email: {employee.Employee.Email}<br/>" +
                               $"Contact No: {employee.Employee.ContactNumber}<br/>" +
                               $"Designation: {employee.Employee.Designation}<br/><br/>" +
                               $"Best regards,<br/>Company Team";
                    _emailService.SendEmail(employee.Employee.Email, subject, body);

                    return RedirectToAction("Employees");
                }

                ViewBag.BranchID = new SelectList(await _branchRepository.GetByCompanyAsync(_sessionHelper.CompanyID), "BranchID", "BranchName", employee.Employee.BranchID);
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
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            var salary = new SalaryMV
            {
                SalaryMonth = DateTime.Now.AddMonths(-1).ToString("MMMM"),
                SalaryYear = DateTime.Now.AddMonths(-1).ToString("yyyy")
            };

            return View(salary);
        }

        [HttpPost]
        public ActionResult EmployeeSalary(SalaryMV salary)
        {
            Session["SalaryMessage"] = string.Empty;

            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                int companyID = Convert.ToInt32(Session["CompanyID"]);
                var employee = _db.tblEmployee.FirstOrDefault(p => p.TIN == salary.TIN);

                if (employee != null)
                {
                    salary.EmployeeID = employee.EmployeeID;
                    salary.EmployeeName = employee.Name;
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
        public async Task<ActionResult> EmployeeSalaryConfirm(SalaryMV salary)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                salary.SalaryMonth = salary.SalaryMonth.ToLower();

                int branchID = Convert.ToInt32(Session["BranchID"]);
                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int userID = Convert.ToInt32(Session["UserID"]);

                var emp = _db.tblPayroll.FirstOrDefault(p => p.EmployeeID == salary.EmployeeID &&
                                                              p.BranchID == branchID &&
                                                              p.CompanyID == companyID &&
                                                              p.SalaryMonth == salary.SalaryMonth &&
                                                              p.SalaryYear == salary.SalaryYear);

                if (emp == null)
                {
                    string invoiceNo = $"ESA{DateTime.Now:yyyyMMddHHmmss}{DateTime.Now.Millisecond}";
                    if (ModelState.IsValid)
                    {
                        string message = await _salaryTransaction.Confirm(salary.EmployeeID, salary.TransferAmount, userID, branchID, companyID, invoiceNo, salary.SalaryMonth, salary.SalaryYear);
                        if (message.Contains("Succeed"))
                        {
                            Session["SalaryMessage"] = message;
                            int payrollNo = _db.tblPayroll.Any() ? _db.tblPayroll.Max(p => p.PayrollID) : 0;
                            return RedirectToAction("PrintSalaryInvoice", new { id = payrollNo });
                        }
                        else
                        {
                            Session["SalaryMessage"] = Resources.Messages.SalaryIsAlreadyPaid;
                        }
                    }
                }
                else
                {
                    Session["SalaryMessage"] = Resources.Messages.PleaseReLoginAndTryAgain;
                }

                return RedirectToAction("EmployeeSalary");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult SalaryHistory()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);

                var salaryList = _db.tblPayroll.Where(p => p.BranchID == branchID && p.CompanyID == companyID)
                                               .OrderByDescending(p => p.PayrollID)
                                               .ToList();

                return View(salaryList);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult PrintSalaryInvoice(int id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                var salary = _db.tblPayroll.FirstOrDefault(p => p.PayrollID == id);
                return View(salary);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}