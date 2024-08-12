using CloudERP.Helpers;
using CloudERP.Models;
using DatabaseAccess;
using System;
using System.Linq;
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

            return View(new RegistrationMV());
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
                    // Add company
                    var company = new tblCompany()
                    {
                        Name = model.CompanyName,
                        Logo = "~/Content/CompanyLogo/erp-logo.png",
                        Description = string.Empty
                    };
                    _db.tblCompany.Add(company);
                    _db.SaveChanges();

                    // Add branch
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

                    // Check if the user already exists
                    if (_db.tblUser.Any(u => u.UserName == model.UserName || u.Email == model.EmployeeEmail))
                    {
                        ModelState.AddModelError("", "A user with this username or email already exists.");
                        return View(model);
                    }

                    // Add user
                    string hashedPassword = PasswordHelper.HashPassword(model.EmployeeContactNo, out string salt);

                    var user = new tblUser()
                    {
                        ContactNo = model.EmployeeContactNo,
                        Email = model.EmployeeEmail,
                        FullName = model.EmployeeName,
                        IsActive = true,
                        Password = hashedPassword,
                        ResetPasswordCode = null,
                        LastPasswordResetRequest = null,
                        ResetPasswordExpiration = null,
                        Salt = salt,
                        UserName = model.UserName,
                        UserTypeID = 2
                    };
                    _db.tblUser.Add(user);
                    _db.SaveChanges();

                    // Check if the employee already exists
                    if (_db.tblEmployee.Any(e => e.Email == model.EmployeeEmail && e.CompanyID == company.CompanyID))
                    {
                        ModelState.AddModelError("", "An employee with this email already exists for this company.");
                        return View(model);
                    }

                    // Add employee
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
                        Description = string.Empty,
                        IsFirstLogin = true,
                        Photo = "~/Content/EmployeePhoto/Default/default.png"
                    };
                    _db.tblEmployee.Add(employee);
                    _db.SaveChanges();

                    // Send email
                    var emailService = new EmailService();
                    var subject = "Welcome to the Company";
                    var body = $"Hello {employee.Name},\n\n" +
                               $"Your registration is successful. Here are your details:\n" +
                               $"Name: {employee.Name}\n" +
                               $"Email: {employee.Email}\n" +
                               $"Contact No: {employee.ContactNo}\n\n" +
                               $"Best regards,\nCompany Team";
                    emailService.SendEmail(employee.Email, subject, body);

                    ViewBag.Message = "Registration Successful!";
                    return RedirectToAction("Login", "Home");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An unexpected error occurred during registration: " + ex.Message;
                    return RedirectToAction("EP500", "EP");
                }
            }

            ViewBag.Message = "Please provide correct details.";
            return View(model);
        }
    }
}
