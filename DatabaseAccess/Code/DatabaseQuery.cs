using System.Data;
using System.Data.SqlClient;

namespace DatabaseAccess.Code
{
    public class DatabaseQuery
    {
        public static SqlConnection conn;

        public static SqlConnection ConnOpen()
        {
            if (conn == null)
            {
                var connectionStringSetting = "data source=localhost\\sqlexpress;initial catalog=CloudErpV1;integrated security=True;";
                conn = new SqlConnection(connectionStringSetting);
            }

            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }

            return conn;
        }

        public static void Insert(string query, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection("data source=localhost\\sqlexpress;initial catalog=CloudErpV1;integrated security=True;"))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool Update(string query)
        {
            try
            {
                int noofrows = 0;
                SqlCommand cmb = new SqlCommand(query, ConnOpen());
                noofrows = cmb.ExecuteNonQuery();

                return noofrows > 0;
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
                int noofrows = 0;
                SqlCommand cmb = new SqlCommand(query, ConnOpen());
                noofrows = cmb.ExecuteNonQuery();

                return noofrows > 0;
            }
            catch
            {
                return false;
            }
        }

        public static DataTable Retrive(string query)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(query, ConnOpen());
                da.Fill(dt);
                return dt;
            }
            catch
            {
                return null;
            }
        }
    }
}