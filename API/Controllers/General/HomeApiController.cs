using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Services.Facades;

namespace API.Controllers
{
    [RoutePrefix("api/home")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HomeApiController : ApiController
    {
        private readonly HomeFacade _homeFacade;

        public HomeApiController(HomeFacade homeFacade)
        {
            _homeFacade = homeFacade ?? throw new ArgumentNullException(nameof(HomeFacade));
        }

        [HttpGet, Route("dashboard")]
        public async Task<IHttpActionResult> GetDashboardValues(int branchId, int companyId)
        {
            try
            {
                var dashboardValues = await _homeFacade.DashboardService.GetDashboardValues(branchId, companyId);
                return Ok(dashboardValues);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("currencies")]
        public async Task<IHttpActionResult> GetCurrencies()
        {
            try
            {
                var defaultCurrency = ConfigurationManager.AppSettings["DefaultCurrency"] ?? "USD";
                var rates = await _homeFacade.CurrencyService.GetExchangeRatesAsync(defaultCurrency);
                var currencies = rates.Keys.Select(k => new { Code = k, Name = k, Rate = rates[k].ToString("F2") });
                return Ok(currencies);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}