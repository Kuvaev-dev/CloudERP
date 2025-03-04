using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace CloudERP.Helpers
{
    public class HttpClientHelper
    {
        private readonly HttpClient _client;
        private readonly SessionHelper _sessionHelper;
        private readonly IConfiguration _configuration;

        public HttpClientHelper(
            HttpClient client,
            SessionHelper sessionHelper,
            IConfiguration configuration)
        {
            _client = client ?? throw new ArgumentException(nameof(client));
            _sessionHelper = sessionHelper ?? throw new ArgumentException(nameof(sessionHelper));
            _configuration = configuration ?? throw new ArgumentException(nameof(configuration));
            _client.BaseAddress = new Uri(_configuration["ApiUri"]);
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _sessionHelper.Token);
                
                var response = await _client.GetAsync(endpoint).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<T>(responseString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET {endpoint} failed: {ex.Message}");
                return default;
            }
        }

        public async Task<bool> PostAsync(string endpoint, object data)
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _sessionHelper.Token);

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(endpoint, content).ConfigureAwait(false);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"POST {endpoint} failed: {ex.Message}");
                return false;
            }
        }

        public async Task<T?> PostAndReturnAsync<T>(string endpoint, object data)
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _sessionHelper.Token);

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(endpoint, content).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"POST {endpoint} failed with status {response.StatusCode}");
                    return default;
                }

                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<T>(responseString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"POST {endpoint} failed: {ex.Message}");
                return default;
            }
        }

        public async Task<bool> PostMultipartAsync(string endpoint, MultipartFormDataContent content)
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _sessionHelper.Token);

                var response = await _client.PostAsync(endpoint, content).ConfigureAwait(false);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"POST {endpoint} failed: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> PutAsync(string endpoint, object data)
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _sessionHelper.Token);

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync(endpoint, content).ConfigureAwait(false);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PUT {endpoint} failed: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> PutMultipartAsync(string endpoint, MultipartFormDataContent content)
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _sessionHelper.Token);

                var response = await _client.PutAsync(endpoint, content).ConfigureAwait(false);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"POST {endpoint} failed: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _sessionHelper.Token);

                var response = await _client.DeleteAsync(endpoint).ConfigureAwait(false);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DELETE {endpoint} failed: {ex.Message}");
                return false;
            }
        }
    }
}
