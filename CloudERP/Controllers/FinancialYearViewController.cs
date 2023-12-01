using DatabaseAccess;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class FinancialYearViewController : Controller
    {
        private readonly CloudDBEntities db = new CloudDBEntities();

        // GET: FinancialYear
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userID = 0;
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var tblFinancialYear = db.tblFinancialYear;
            return View(tblFinancialYear.ToList());
        }
    }
}