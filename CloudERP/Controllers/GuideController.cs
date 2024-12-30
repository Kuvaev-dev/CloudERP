using CloudERP.Helpers;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class GuideController : Controller
    {
        private readonly SessionHelper _sessionHelper;

        public GuideController(SessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
        }

        // GET: Guide
        public ActionResult AdminMenuGuide()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login");

            return View();
        }

        public ActionResult MainBranchEmployeeGuide()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login");

            return View();
        }

        public ActionResult EmployeeGuide()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login");

            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login");

            return View();
        }
    }
}