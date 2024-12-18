using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DatabaseAccess.Code
{
    public class DatabaseQuery
    {
        private static readonly string connectionString = "data source=localhost\\sqlexpress;initial catalog=CloudErp;integrated security=True;";

        public async Task<SqlConnection> ConnOpen()
        {
            var connection = new SqlConnection(connectionString);
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