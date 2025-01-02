using Domain.Services.Interfaces;
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
            var connection = new SqlConnection(_connectionStringProvider.GetConnectionString("CloudDBEntities"));
            await connection.OpenAsync();
            return connection;
        }

        public async Task Insert(string query, params SqlParameter[] parameters)
        {
            using (var connection = await ConnOpen())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<bool> Update(string query)
        {
            try
            {
                using (var connection = await ConnOpen())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        int noOfRows = await command.ExecuteNonQueryAsync();
                        return noOfRows > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Delete(string query)
        {
            try
            {
                using (var connection = await ConnOpen())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        int noOfRows = await command.ExecuteNonQueryAsync();
                        return noOfRows > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<DataTable> Retrive(string query)
        {
            var dt = new DataTable();
            using (var connection = await ConnOpen())
            {
                using (var cmd = new SqlCommand(query, connection))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }
    }
}