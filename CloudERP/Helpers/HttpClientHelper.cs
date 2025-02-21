using Newtonsoft.Json;
using System.Text;

namespace CloudERP.Helpers
{
    public class HttpClientHelper : IDisposable
    {
        private readonly HttpClient _client;

        public HttpClientHelper(string baseUrl = "http://localhost:5145/api")
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/") };
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var response = await _client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        public async Task<bool> PostAsync<T>(string endpoint, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(endpoint, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<T?> PostAndReturnAsync<T>(string endpoint, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(endpoint, content);
            if (!response.IsSuccessStatusCode) return default;

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseString);
        }

        public async Task<bool> PutAsync<T>(string endpoint, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync(endpoint, content);
            return response.IsSuccessStatusCode;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}