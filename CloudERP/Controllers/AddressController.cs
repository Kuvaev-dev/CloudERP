using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class AddressController : Controller
    {
        private readonly string apiKey = System.Configuration.ConfigurationManager.AppSettings["GeoapifyApiKey"];

        public async Task<ActionResult> Autocomplete(string query)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"https://api.geoapify.com/v1/geocode/autocomplete?text={query}&apiKey={apiKey}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Content(content, "application/json");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new HttpStatusCodeResult(response.StatusCode, $"{Resources.Messages.ErrorFetchingDataFromGeoapifyAPI} {errorContent}");
                }
            }
        }
    }
}