using Appical.Domain.Configuration.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Appical.Data.Helper
{
    public static class RegisterServicesHelper
    {
        public static IServiceCollection RegisterConfiguration(this IServiceCollection services, IApiConfiguration config)
        {
            services.AddSingleton(config);
            services.AddSingleton<IDatabaseConfig>(config.Database);

            return services;
        }
    }
}
