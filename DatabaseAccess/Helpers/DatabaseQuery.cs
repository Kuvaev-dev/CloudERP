using Domain.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DatabaseAccess.Helpers
{
    public class DatabaseQuery
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        public DatabaseQuery(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        public async Task<SqlConnection> ConnOpen()
        {
            var connection = new SqlConnection(_connectionStringProvider.GetConnectionString("CloudDBDirect"));
            await connection.OpenAsync();
            return connection;
        }

        public async Task Insert(string query, params SqlParameter[] parameters)
        {
            await ExecuteNonQuery(query, parameters);
        }

        public async Task<bool> Update(string query, params SqlParameter[] parameters)
        {
            return await ExecuteNonQuery(query, parameters);
        }

        public async Task<bool> Delete(string query, params SqlParameter[] parameters)
        {
            return await ExecuteNonQuery(query, parameters);
        }

        private async Task<bool> ExecuteNonQuery(string query, SqlParameter[] parameters)
        {
            try
            {
                using (var connection = await ConnOpen())
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);

                    int noOfRows = await command.ExecuteNonQueryAsync();
                    return noOfRows > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<DataTable> Retrieve(string query, params SqlParameter[] parameters)
        {
            var dt = new DataTable();
            using (var connection = await ConnOpen())
            using (var cmd = new SqlCommand(query, connection))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }
    }
}