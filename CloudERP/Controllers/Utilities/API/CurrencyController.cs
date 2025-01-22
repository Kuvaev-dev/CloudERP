using Domain.Interfaces;
using System.Configuration;
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

        public async Task<ActionResult> SetCurrency(string currencyCode)
        {
            if (string.IsNullOrEmpty(currencyCode))
            {
                currencyCode = ConfigurationManager.AppSettings["DefaultCurrency"];
            }

            Session["SelectedCurrency"] = currencyCode;

            var rates = await _currencyService.GetExchangeRatesAsync(currencyCode);

            ViewBag.Currencies = rates.Keys.Select(k => new { Code = k, Name = k }).ToList();

            return RedirectToAction("Index", "Home");
        }
    }
}