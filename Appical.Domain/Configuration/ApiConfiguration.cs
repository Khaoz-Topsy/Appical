using Appical.Domain.Configuration.Interface;

namespace Appical.Domain.Configuration
{
    public class ApiConfiguration: IApiConfiguration
    {
        public DatabaseConfig Database { get; set; }
    }
}
