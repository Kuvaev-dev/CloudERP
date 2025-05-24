using Domain.UtilsAccess;
using Microsoft.Data.SqlClient;

namespace DatabaseAccess.Helpers
{
    public class DatabaseQuery : IDatabaseQuery
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        public DatabaseQuery(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        public async Task<object> ConnOpenAsync()
        {
            var connection = new SqlConnection(_connectionStringProvider.GetConnectionString("CloudDBDirect"));
            await connection.OpenAsync();
            return connection;
        }

        public async Task<bool> ExecuteNonQueryAsync(string query, params object[] parameters)
        {
            try
            {
                using SqlConnection connection = await ConnOpenAsync() as SqlConnection;
                using var command = new SqlCommand(query, connection);
                if (parameters != null)
                    command.Parameters.AddRange(parameters as SqlParameter[]);

                int affectedRows = await command.ExecuteNonQueryAsync();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<Dictionary<string, object>>> RetrieveAsync(string query, params object[] parameters)
        {
            var results = new List<Dictionary<string, object>>();

            try
            {
                using SqlConnection connection = await ConnOpenAsync() as SqlConnection;
                using var command = new SqlCommand(query, connection);
                if (parameters != null)
                    command.Parameters.AddRange(parameters as SqlParameter[]);

                using var reader = await command.ExecuteReaderAsync();
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
            catch (Exception ex)
            {
                return [];
            }

            return results;
        }
    }
}