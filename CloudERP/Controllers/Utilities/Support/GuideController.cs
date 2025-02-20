using CloudERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Utilities.Support
{
    public class GuideController : Controller
    {
        private readonly SessionHelper _sessionHelper;

        public GuideController(SessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
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