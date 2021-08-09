using Appical.Domain.Configuration.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Appical.Persistence.Helper
{
    public static class SetupContextHelper
    {
        public static void SetUpEntityFramework(this IServiceCollection services, IDatabaseConfig databaseConfig)
        {
            services.AddDbContext<AppicalContext>(options => options
                .UseLazyLoadingProxies()
                .UseSqlServer(databaseConfig.ConnectionString, dbOptions => dbOptions.MigrationsAssembly("Appical.Api"))
            );
        }
    }
}
