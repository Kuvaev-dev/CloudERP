using CloudERP.Helpers;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class BranchEmployeeController : Controller
    {
        private CloudDBEntities db = new CloudDBEntities();

        // GET: Employee
        public ActionResult Employee()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            int branchID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            var tblEmployee = db.tblEmployee.Where(c => c.CompanyID == companyID && c.BranchID == branchID);
            return View(tblEmployee);
        }

        // GET: EmployeeRegistration
        public ActionResult EmployeeRegistration()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
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

            int companyID = 0;
            int branchID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            employee.BranchID = branchID;
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
            var employee = db.tblEmployee.Find(id);
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

            int companyID = 0;
            int branchID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
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
                        db.Entry(employee).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Employee");
            }

            return View(employee);
        }
    }
}