using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace API.Controllers.Utilities
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AddressApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _apiUrl;

        public AddressApiController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKey = _configuration["GeoapifyApi:Key"] ?? throw new ArgumentNullException("Geoapify Api Key is missing in configuration.");
            _apiUrl = _configuration["GeoapifyApi:BaseUrl"] ?? throw new ArgumentNullException("Geoapify Api Url is missing in configuration.");
        }

        [HttpGet]
        public async Task<IActionResult> Autocomplete([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query parameter is required.");

            try
            {
                using var client = new HttpClient();
                var encodedQuery = Uri.EscapeDataString(query);
                var response = await client.GetAsync($"{_apiUrl}/geocode/autocomplete?text={encodedQuery}&apiKey={_apiKey}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Content(content, "application/json");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Error fetching data from Geoapify API: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAddressByCoordinates([FromQuery] double latitude, [FromQuery] double longitude)
        {
            try
            {
                using var client = new HttpClient();

                var latStr = latitude.ToString(CultureInfo.InvariantCulture);
                var lonStr = longitude.ToString(CultureInfo.InvariantCulture);

                var url = $"{_apiUrl}/geocode/reverse?lat={latStr}&lon={lonStr}&apiKey={_apiKey}";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Content(content, "application/json");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Error fetching data from Geoapify API: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}