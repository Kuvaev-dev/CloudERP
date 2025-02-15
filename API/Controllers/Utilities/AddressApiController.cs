using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace API.Controllers
{
    [RoutePrefix("api/address")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AddressApiController : Controller
    {
        private readonly string apiKey = System.Configuration.ConfigurationManager.AppSettings["GeoapifyApiKey"];
        private readonly string apiUrl = System.Configuration.ConfigurationManager.AppSettings["GeoapifyApiUrl"];

        [HttpGet, Route("")]
        public async Task<ActionResult> Autocomplete(string query)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{apiUrl}/autocomplete?text={query}&apiKey={apiKey}");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        return Content(content, "application/json");
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return new HttpStatusCodeResult(response.StatusCode, $"{Localization.CloudERP.Messages.Messages.ErrorFetchingDataFromGeoapifyAPI} {errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpGet, Route("")]
        public async Task<ActionResult> GetAddressByCoordinates(double latitude, double longitude)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{apiUrl}/reverse?lat={latitude}&lon={longitude}&apiKey={apiKey}");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        return Content(content, "application/json");
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return new HttpStatusCodeResult(response.StatusCode, $"{Localization.CloudERP.Messages.Messages.ErrorFetchingDataFromGeoapifyAPI} {errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}