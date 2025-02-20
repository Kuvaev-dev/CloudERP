using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class AddressApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _apiUrl;

        public AddressApiController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKey = _configuration["GeoapifyApiKey"] ?? throw new ArgumentNullException("GeoapifyApiKey is missing in configuration.");
            _apiUrl = _configuration["GeoapifyApiUrl"] ?? throw new ArgumentNullException("GeoapifyApiUrl is missing in configuration.");
        }

        [HttpGet("autocomplete")]
        public async Task<IActionResult> Autocomplete([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query parameter is required.");

            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync($"{_apiUrl}/autocomplete?text={query}&apiKey={_apiKey}");

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

        [HttpGet("reverse")]
        public async Task<IActionResult> GetAddressByCoordinates([FromQuery] double latitude, [FromQuery] double longitude)
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync($"{_apiUrl}/reverse?lat={latitude}&lon={longitude}&apiKey={_apiKey}");

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