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

        // GET: CompanyRegistration
        public ActionResult RegistrationForm()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult RegistrationForm(string UserName, string Password, string CPassword,
                                             string EName, string EContactNo, string EEmail,
                                             string ECNIC, string EDesignation, float EMonthlySalary,
                                             string EAddress, string CName, string BranchName,
                                             string BranchContact, string BranchAddress)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password) &&
                        !string.IsNullOrEmpty(CPassword) && !string.IsNullOrEmpty(EName) &&
                        !string.IsNullOrEmpty(EContactNo) && !string.IsNullOrEmpty(EEmail) &&
                        !string.IsNullOrEmpty(ECNIC) && !string.IsNullOrEmpty(EDesignation) &&
                        EMonthlySalary > 0 && !string.IsNullOrEmpty(EAddress) &&
                        !string.IsNullOrEmpty(CName) && !string.IsNullOrEmpty(BranchName) &&
                        !string.IsNullOrEmpty(BranchContact) && !string.IsNullOrEmpty(BranchAddress))
                {
                    var company = new tblCompany()
                    {
                        Name = CName,
                        Logo = string.Empty
                    };
                    _db.tblCompany.Add(company);
                    _db.SaveChanges();

                    var branch = new tblBranch()
                    {
                        BranchAddress = BranchAddress,
                        BranchContact = BranchContact,
                        BranchName = BranchName,
                        BranchTypeID = 1,
                        CompanyID = company.CompanyID,
                        BrchID = null
                    };
                    _db.tblBranch.Add(branch);
                    _db.SaveChanges();

                    var user = new tblUser()
                    {
                        ContactNo = EContactNo,
                        Email = EEmail,
                        FullName = EName,
                        IsActive = true,
                        Password = Password,
                        UserName = UserName,
                        UserTypeID = 2
                    };
                    _db.tblUser.Add(user);
                    _db.SaveChanges();

                    var employee = new tblEmployee()
                    {
                        Address = EAddress,
                        BranchID = branch.BranchID,
                        CNIC = ECNIC,
                        CompanyID = company.CompanyID,
                        ContactNo = EContactNo,
                        Designation = EDesignation,
                        Email = EEmail,
                        MonthlySalary = EMonthlySalary,
                        UserID = user.UserID,
                        Name = EName,
                        Description = string.Empty
                    };
                    _db.tblEmployee.Add(employee);
                    _db.SaveChanges();

                    ViewBag.Message = "Registration Successfully!";
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    ViewBag.Message = "Please Provide Correct Details!";
                    return View("RegistrationForm");
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Please Contact to Administrator!";
                return View();
            }
        }
    }
}