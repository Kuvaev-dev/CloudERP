using CloudERP.Helpers;
using CloudERP.Models;
using DatabaseAccess;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class CompanyEmployeeController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SalaryTransaction _salaryTransaction;

        public CompanyEmployeeController(CloudDBEntities db)
        {
            _db = db;
            _salaryTransaction = new SalaryTransaction(_db);
        }

        // GET: Employees
        public ActionResult Employees()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                int companyID = Convert.ToInt32(Session["CompanyID"]);
                var tblEmployee = _db.tblEmployee.Where(c => c.CompanyID == companyID).ToList();
                return View(tblEmployee);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving employees: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult EmployeeRegistration()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                int companyID = Convert.ToInt32(Session["CompanyID"]);
                ViewBag.BranchID = new SelectList(_db.tblBranch.Where(b => b.CompanyID == companyID), "BranchID", "BranchName", 0);
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while loading the employee registration page: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeRegistration(tblEmployee employee)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                employee.CompanyID = Convert.ToInt32(Session["CompanyID"]);
                employee.UserID = null;
                employee.RegistrationDate = DateTime.Now;

                if (ModelState.IsValid)
                {
                    _db.tblEmployee.Add(employee);
                    _db.SaveChanges();

                    if (employee.LogoFile != null)
                    {
                        var folder = "~/Content/EmployeePhoto";
                        var file = $"{employee.EmployeeID}.jpg";

                        var response = FileHelper.UploadPhoto(employee.LogoFile, folder, file);
                        if (response)
                        {
                            var filePath = Server.MapPath($"{folder}/{file}");
                            if (System.IO.File.Exists(filePath))
                            {
                                employee.Photo = $"{folder}/{file}";
                            }
                            else
                            {
                                employee.Photo = "~/Content/EmployeePhoto/Default/default.png";
                            }
                            _db.Entry(employee).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                        else
                        {
                            employee.Photo = "~/Content/EmployeePhoto/Default/default.png";
                            _db.Entry(employee).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        employee.Photo = "~/Content/EmployeePhoto/Default/default.png";
                        _db.Entry(employee).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    // Send email
                    var emailService = new EmailService();
                    var subject = "Employee Registration Successful";
                    var body = $"<strong>Dear {employee.Name},</strong><br/><br/>" +
                               $"Your registration is successful. Here are your details:<br/>" +
                               $"Name: {employee.Name}<br/>" +
                               $"Email: {employee.Email}<br/>" +
                               $"Contact No: {employee.ContactNo}<br/>" +
                               $"Designation: {employee.Designation}<br/><br/>" +
                               $"Best regards,<br/>Company Team";
                    emailService.SendEmail(employee.Email, subject, body);

                    return RedirectToAction("Employees");
                }

                ViewBag.BranchID = new SelectList(_db.tblBranch.Where(b => b.CompanyID == employee.CompanyID), "BranchID", "BranchName", employee.BranchID);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while registering the employee: " + ex.Message;
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
                    Session["SalaryMessage"] = "Record Not Found";
                }

                salary.SalaryMonth = DateTime.Now.AddMonths(-1).ToString("MMMM");
                salary.SalaryYear = DateTime.Now.AddMonths(-1).ToString("yyyy");

                return View(salary);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while processing the salary: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult EmployeeSalaryConfirm(SalaryMV salary)
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
                        string message = _salaryTransaction.Confirm(salary.EmployeeID, salary.TransferAmount, userID, branchID, companyID, invoiceNo, salary.SalaryMonth, salary.SalaryYear);
                        if (message.Contains("Succeed"))
                        {
                            Session["SalaryMessage"] = message;
                            int payrollNo = _db.tblPayroll.Any() ? _db.tblPayroll.Max(p => p.PayrollID) : 0; // Check if tblPayroll has records
                            return RedirectToAction("PrintSalaryInvoice", new { id = payrollNo });
                        }
                        else
                        {
                            Session["SalaryMessage"] = "Salary is Already Paid";
                        }
                    }
                }
                else
                {
                    Session["SalaryMessage"] = "Please Re-Login and Try Again";
                }

                return RedirectToAction("EmployeeSalary");
            }
            catch (Exception)
            {
                Session["SalaryMessage"] = "Some Unexpected Issue is Occurred. Please Try Again";
                return RedirectToAction("EmployeeSalary");
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
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving salary history: " + ex.Message;
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
                TempData["ErrorMessage"] = "An unexpected error occurred while printing the salary invoice: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}