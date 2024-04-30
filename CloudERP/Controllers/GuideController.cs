using System;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class GuideController : Controller
    {
        // GET: Guide
        public ActionResult AdminMenuGuide()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        public ActionResult MainBranchEmployeeGuide()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        public ActionResult EmployeeGuide()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login");
            }

            return View();
        }
    }
}