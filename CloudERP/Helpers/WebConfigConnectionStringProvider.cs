using System;
using System.Configuration;
using Utils.Interfaces;

namespace CloudERP.Helpers
{
    public class WebConfigConnectionStringProvider : IConnectionStringProvider
    {
        public string GetConnectionString(string name)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Connection string '{name}' is not defined in the configuration file.");
            }
            return connectionString;
        }
    }
}