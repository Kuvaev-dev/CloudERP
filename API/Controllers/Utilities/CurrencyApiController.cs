using System.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;

namespace API.Controllers
{
    [RoutePrefix("api/currency")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CurrencyApiController : ApiController
    {
        [HttpGet, Route("")]
        public IHttpActionResult SetCurrency(string currencyCode)
        {
            if (string.IsNullOrEmpty(currencyCode))
            {
                currencyCode = ConfigurationManager.AppSettings["DefaultCurrency"] ?? "UAH";
            }

            return Ok(currencyCode);
        }
    }
}