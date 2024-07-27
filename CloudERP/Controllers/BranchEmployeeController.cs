using CloudERP.Helpers;
using DatabaseAccess;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class BranchEmployeeController : Controller
    {
        private readonly CloudDBEntities _db;

        public BranchEmployeeController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: Employee
        public ActionResult Employee()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            var tblEmployee = _db.tblEmployee.Where(c => c.CompanyID == companyID && c.BranchID == branchID);

            return View(tblEmployee);
        }

        // GET: EmployeeRegistration
        public ActionResult EmployeeRegistration()
        {
            return View();
        }

        // POST: EmployeeRegistration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeRegistration(tblEmployee employee)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            employee.BranchID = branchID;
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

                return RedirectToAction("Employee");
            }

            return View(employee);
        }

        // GET: EmployeeUpdation
        public ActionResult EmployeeUpdation(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var employee = _db.tblEmployee.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            return View(employee);
        }

        // POST: EmployeeUpdation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeUpdation(tblEmployee employee)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            employee.BranchID = branchID;
            employee.CompanyID = companyID;
            employee.UserID = null;

            if (ModelState.IsValid)
            {
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

                return RedirectToAction("Employee");
            }

            return View(employee);
        }

        public ActionResult ViewProfile(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                if (id == null)
                {
                    return RedirectToAction("EP500", "EP");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                
                var employee = _db.tblEmployee.Where(c => c.CompanyID == companyID && c.EmployeeID == id).FirstOrDefault();
                if (employee == null)
                {
                    return RedirectToAction("EP404", "EP");
                }

                return View(employee);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}