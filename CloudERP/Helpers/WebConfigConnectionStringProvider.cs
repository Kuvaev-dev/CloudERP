using Domain.Interfaces;
using System.Configuration;

namespace CloudERP.Helpers
{
    public class WebConfigConnectionStringProvider : IConnectionStringProvider
    {
        public string GetConnectionString(string name)
            => ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
    }
}