using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class AddressController : Controller
    {
        private readonly string apiKey = "3fd24601a56441aaaa626d013be53eac";

        public async Task<ActionResult> Autocomplete(string query)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"https://api.geoapify.com/v1/geocode/autocomplete?text={query}&apiKey={apiKey}");
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
        }
    }
}