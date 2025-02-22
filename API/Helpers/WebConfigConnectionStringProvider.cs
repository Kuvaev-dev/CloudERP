using Utils.Interfaces;

namespace API.Helpers
{
    public class WebConfigConnectionStringProvider : IConnectionStringProvider
    {
        private readonly IConfiguration _configuration;

        public WebConfigConnectionStringProvider(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string GetConnectionString(string name)
        {
            var connectionString = _configuration.GetConnectionString(name);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Connection string '{name}' is not defined in the configuration file.");
            }
            return connectionString;
        }
    }
}