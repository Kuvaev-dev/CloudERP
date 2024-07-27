using CloudERP.Helpers;
using CloudERP.Models;
using DatabaseAccess;
using DatabaseAccess.Code;
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

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            var tblEmployee = _db.tblEmployee.Where(c => c.CompanyID == companyID).ToList();

            return View(tblEmployee);
        }

        public ActionResult EmployeeRegistration()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            ViewBag.BranchID = new SelectList(_db.tblBranch.Where(b => b.CompanyID == companyID), "BranchID", "BranchName", 0);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeRegistration(tblEmployee employee)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            employee.CompanyID = Convert.ToInt32(Session["CompanyID"]);
            employee.UserID = null;

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
                        employee.Photo = $"{folder}/{file}";
                        _db.Entry(employee).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }

                return RedirectToAction("Employees");
            }

            ViewBag.BranchID = new SelectList(_db.tblBranch.Where(b => b.CompanyID == employee.CompanyID), "BranchID", "BranchName", employee.BranchID);
            
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

                if (emp != null)
                {
                    string invoiceNo = $"ESA{DateTime.Now:yyyyMMddHHmmss}{DateTime.Now.Millisecond}";
                    if (ModelState.IsValid)
                    {
                        string message = _salaryTransaction.Confirm(salary.EmployeeID, salary.TransferAmount, userID, branchID, companyID, invoiceNo, salary.SalaryMonth, salary.SalaryYear);
                        if (message.Contains("Succeed"))
                        {
                            Session["SalaryMessage"] = message;
                            int payrollNo = _db.tblPayroll.Max(p => p.PayrollID);
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
            catch
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

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            var salaryList = _db.tblPayroll.Where(p => p.BranchID == branchID && p.CompanyID == companyID)
                                           .OrderByDescending(p => p.PayrollID)
                                           .ToList();

            return View(salaryList);
        }

        public ActionResult PrintSalaryInvoice(int id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            var salary = _db.tblPayroll.FirstOrDefault(p => p.PayrollID == id);
            
            return View(salary);
        }
    }
}