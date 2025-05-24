namespace Domain.UtilsAccess
{
    public interface IDatabaseQuery
    {
        Task<object> ConnOpenAsync();
        Task<bool> ExecuteNonQueryAsync(string query, params object[] parameters);
        Task<List<Dictionary<string, object>>> RetrieveAsync(string query, params object[] parameters);
    }
}
