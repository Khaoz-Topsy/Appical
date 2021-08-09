using Appical.Domain.Configuration.Interface;

namespace Appical.Domain.Configuration
{
    public class DatabaseConfig : IDatabaseConfig
    {
        public string ConnectionString { get; set; }
    }
}
