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
        private readonly SalaryTransaction salaryTransaction = new SalaryTransaction();

        public CompanyEmployeeController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: Employees
        public ActionResult Employees()
        {
            if(string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            var tblEmployee = _db.tblEmployee.Where(c => c.CompanyID == companyID);
            return View(tblEmployee);
        }

        public ActionResult EmployeeRegistration()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            ViewBag.BranchID = new SelectList(_db.tblBranch.Where(b => b.CompanyID == companyID), "BranchID", "BranchName", 0);
            return View();
        }

        // POST: Employees
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeRegistration(tblEmployee employee)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = 0;
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            employee.CompanyID = companyID;
            employee.UserID = null;

            if (ModelState.IsValid)
            {
                _db.tblEmployee.Add(employee);
                _db.SaveChanges();

                if (employee.LogoFile != null)
                {
                    var folder = "~/Content/EmployeePhoto";
                    var file = string.Format("{0}.jpg", employee.EmployeeID);
                    var response = FileHelper.UploadPhoto(employee.LogoFile, folder, file);
                    if (response)
                    {
                        var picture = string.Format("{0}/{1}", folder, file);
                        employee.Photo = picture;
                        _db.Entry(employee).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }

                return RedirectToAction("Employees");
            }

            return View(employee);
        }

        public ActionResult EmployeeSalary()
        {
            if (Session["SalaryMessage"] == null)
            {
                Session["SalaryMessage"] = string.Empty;
            }
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            int branchID = 0;
            int userID = 0;
            var salary = new SalaryMV();
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            salary.SalaryMonth = DateTime.Now.AddMonths(-1).ToString("MMMM");
            salary.SalaryYear = DateTime.Now.AddMonths(-1).ToString("yyyy");
            return View(salary);
        }

        [HttpPost]
        public ActionResult EmployeeSalary(SalaryMV salary) // CNIC
        {
            Session["SalaryMessage"] = string.Empty;
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            int branchID = 0;
            int userID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var employee = _db.tblEmployee.Where(p => p.TIN == salary.TIN).FirstOrDefault();
            salary.SalaryMonth = DateTime.Now.AddMonths(-1).ToString("MMMM");
            salary.SalaryYear = DateTime.Now.AddMonths(-1).ToString("yyyy");
            if (employee != null)
            {
                salary.EmployeeID = employee.EmployeeID;
                salary.EmployeeName = employee.Name;
                salary.Designation = employee.Designation;
                salary.TIN = employee.TIN;
                salary.TransferAmount = employee.MonthlySalary;
                Session["SalaryMessage"] = "";
            }
            else
            {
                Session["SalaryMessage"] = "Record Not Found";
            }
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
                int companyID = 0;
                int branchID = 0;
                int userID = 0;
                branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
                companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
                userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
                salary.SalaryMonth = salary.SalaryMonth.ToLower();
                var emp = _db.tblPayroll.Where(p => p.EmployeeID == salary.EmployeeID && p.BranchID == branchID && p.CompanyID == companyID && p.SalaryMonth == salary.SalaryMonth && p.SalaryYear == salary.SalaryYear).FirstOrDefault();
                if (emp != null)
                {
                    string invoiceNo = "ESA" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                    string message = string.Empty;
                    if (ModelState.IsValid)
                    {
                        message = salaryTransaction.Confirm(salary.EmployeeID, salary.TransferAmount, userID, branchID, companyID, invoiceNo, salary.SalaryMonth, salary.SalaryYear);
                    }
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
                else
                {
                    Session["SalaryMessage"] = "Please Re-Login and Try Again";
                }
                return RedirectToAction("EmployeeSalary");
            }
            catch
            {
                Session["SalaryMessage"] = "Some Unexpected Issue is Occure. Please Try Again";
                return RedirectToAction("EmployeeSalary");
            }
        }

        public ActionResult SalaryHistory()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            int branchID = 0;
            int userID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var salaryList = _db.tblPayroll.Where(p => p.BranchID == branchID && p.CompanyID == companyID).OrderByDescending(p => p.PayrollID).ToList();
            return View(salaryList);
        }

        public ActionResult PrintSalaryInvoice(int id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            
            var salary = _db.tblPayroll.Where(p => p.PayrollID == id).FirstOrDefault();
            return View(salary);
        }
    }
}