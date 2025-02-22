using Microsoft.Data.SqlClient;
using Utils.Interfaces;
using Microsoft.Extensions.Logging;

namespace Utils.Helpers
{
    public class DatabaseQuery
    {
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly ILogger<DatabaseQuery> _logger;

        public DatabaseQuery(IConnectionStringProvider connectionStringProvider, ILogger<DatabaseQuery> logger)
        {
            _connectionStringProvider = connectionStringProvider;
            _logger = logger;
        }

        public async Task<SqlConnection> ConnOpenAsync()
        {
            var connection = new SqlConnection(_connectionStringProvider.GetConnectionString("CloudDBDirect"));
            await connection.OpenAsync();
            return connection;
        }

        public async Task InsertAsync(string query, params SqlParameter[] parameters)
        {
            await ExecuteNonQueryAsync(query, parameters);
        }

        public async Task<bool> UpdateAsync(string query, params SqlParameter[] parameters)
        {
            return await ExecuteNonQueryAsync(query, parameters);
        }

        public async Task<bool> DeleteAsync(string query, params SqlParameter[] parameters)
        {
            return await ExecuteNonQueryAsync(query, parameters);
        }

        private async Task<bool> ExecuteNonQueryAsync(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection connection = await ConnOpenAsync())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                            command.Parameters.AddRange(parameters);

                        int affectedRows = await command.ExecuteNonQueryAsync();
                        return affectedRows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка выполнения запроса: {Query}", query);
                return false;
            }
        }

        public async Task<List<Dictionary<string, object>>> RetrieveAsync(string query, params SqlParameter[] parameters)
        {
            var results = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = await ConnOpenAsync())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                            command.Parameters.AddRange(parameters);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }
                                results.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при выполнении запроса: {Query}", query);
            }

            return results;
        }
    }
}