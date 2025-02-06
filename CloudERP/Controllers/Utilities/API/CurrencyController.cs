using System.Configuration;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class CurrencyController : Controller
    {
        public ActionResult SetCurrency(string currencyCode)
        {
            if (string.IsNullOrEmpty(currencyCode))
            {
                currencyCode = ConfigurationManager.AppSettings["DefaultCurrency"] ?? "USD";
            }

            Session["SelectedCurrency"] = currencyCode;

            return RedirectToAction("Index", "Home");
        }
    }
}