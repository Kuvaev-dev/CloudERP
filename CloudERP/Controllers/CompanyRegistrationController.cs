using CloudERP.Helpers;
using CloudERP.Models;
using DatabaseAccess;
using System;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class CompanyRegistrationController : Controller
    {
        private readonly CloudDBEntities _db;

        public CompanyRegistrationController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: CompanyRegistration/RegistrationForm
        public ActionResult RegistrationForm()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        // POST: CompanyRegistration/RegistrationForm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrationForm(RegistrationMV model)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var company = new tblCompany()
                    {
                        Name = model.CompanyName,
                        Logo = string.Empty
                    };
                    _db.tblCompany.Add(company);
                    _db.SaveChanges();

                    var branch = new tblBranch()
                    {
                        BranchAddress = model.BranchAddress,
                        BranchContact = model.BranchContact,
                        BranchName = model.BranchName,
                        BranchTypeID = 1,
                        CompanyID = company.CompanyID,
                        BrchID = null
                    };
                    _db.tblBranch.Add(branch);
                    _db.SaveChanges();

                    string hashedPassword = PasswordHelper.HashPassword(model.Password, out string salt);

                    var user = new tblUser()
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
                    _db.tblUser.Add(user);
                    _db.SaveChanges();

                    var employee = new tblEmployee()
                    {
                        Address = model.EmployeeAddress,
                        BranchID = branch.BranchID,
                        TIN = model.EmployeeTIN,
                        CompanyID = company.CompanyID,
                        ContactNo = model.EmployeeContactNo,
                        Designation = model.EmployeeDesignation,
                        Email = model.EmployeeEmail,
                        MonthlySalary = model.EmployeeMonthlySalary,
                        UserID = user.UserID,
                        Name = model.EmployeeName,
                        Description = string.Empty // Placeholder for description
                    };
                    _db.tblEmployee.Add(employee);
                    _db.SaveChanges();

                    ViewBag.Message = "Registration Successful!";
                    return RedirectToAction("Login", "Home");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"Registration failed: {ex.Message}";
                    return View(model);
                }
            }

            ViewBag.Message = "Please provide correct details.";
            return View(model);
        }
    }
}
