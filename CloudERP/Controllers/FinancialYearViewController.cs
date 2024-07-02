using DatabaseAccess;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class FinancialYearViewController : Controller
    {
        private readonly CloudDBEntities _db;

        public FinancialYearViewController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: FinancialYear
        public ActionResult Index()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int userID = Convert.ToInt32(Session["UserID"]);
                var tblFinancialYear = _db.tblFinancialYear.ToList();
                return View(tblFinancialYear);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
