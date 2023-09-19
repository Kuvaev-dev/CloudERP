using CloudERP.Helpers;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class CompanyEmployeeController : Controller
    {
        private CloudDBEntities db = new CloudDBEntities();

        // GET: Employees
        public ActionResult Employees()
        {
            if(string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            var tblEmployee = db.tblEmployee.Where(c => c.CompanyID == companyID);
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
            ViewBag.BranchID = new SelectList(db.tblBranch.Where(b => b.CompanyID == companyID), "BranchID", "BranchName", 0);
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
                db.tblEmployee.Add(employee);
                db.SaveChanges();

                if (employee.LogoFile != null)
                {
                    var folder = "~/Content/EmployeePhoto";
                    var file = string.Format("{0}.jpg", employee.EmployeeID);
                    var response = FileHelper.UploadPhoto(employee.LogoFile, folder, file);
                    if (response)
                    {
                        var picture = string.Format("{0}/{1}", folder, file);
                        employee.Photo = picture;
                        db.Entry(employee).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Employees");
            }

            return View(employee);
        }
    }
}