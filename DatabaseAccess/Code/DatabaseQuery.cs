using System.Data;
using System.Data.SqlClient;

namespace DatabaseAccess.Code
{
    public class DatabaseQuery
    {
        private static readonly string connectionString = "data source=localhost\\sqlexpress;initial catalog=CloudErp;integrated security=True;";

        public static SqlConnection ConnOpen()
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        public static void Insert(string query, params SqlParameter[] parameters)
        {
            using (var connection = ConnOpen())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool Update(string query)
        {
            try
            {
                using (var connection = ConnOpen())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        int noOfRows = command.ExecuteNonQuery();
                        return noOfRows > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool Delete(string query)
        {
            try
            {
                using (var connection = ConnOpen())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        int noOfRows = command.ExecuteNonQuery();
                        return noOfRows > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static DataTable Retrive(string query)
        {
            var dt = new DataTable();
            using (var connection = ConnOpen())
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