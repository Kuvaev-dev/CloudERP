namespace Domain.UtilsAccess
{
    public interface IHttpClientHelper
    {
        Task<T?> GetAsync<T>(string endpoint);
        Task<bool> PostAsync(string endpoint, object data);
        Task<T?> PostAndReturnAsync<T>(string endpoint, object data);
        Task<bool> PutAsync(string endpoint, object data);
        Task<bool> DeleteAsync(string endpoint);
    }
}
