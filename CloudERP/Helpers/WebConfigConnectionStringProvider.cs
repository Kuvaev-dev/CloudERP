using Domain.Services;
using Domain.Services.Interfaces;
using System;
using System.Configuration;

namespace CloudERP.Helpers
{
    public class WebConfigConnectionStringProvider : IConnectionStringProvider
    {
        public string GetConnectionString(string name)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[name]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Connection string '{name}' not found.");
            }

            return connectionString;
        }
    }
}