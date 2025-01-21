using Domain.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public async Task<ActionResult> Index()
        {
            var rates = await _currencyService.GetExchangeRatesAsync("UAH");
            ViewBag.Currencies = rates.Keys.Select(k => new { Code = k, Name = k });
            return View();
        }

        [HttpPost]
        public ActionResult SetCurrency(string currencyCode)
        {
            if (!string.IsNullOrEmpty(currencyCode))
            {
                HttpContext.Session["SelectedCurrency"] = currencyCode;
            }
            return RedirectToAction("Index");
        }
    }
}