using Appical.Persistence.Repository;
using Appical.Persistence.Repository.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Appical.Persistence.Helper
{
    public static class RegisterServicesHelper
    {
        public static IServiceCollection RegisterPersistenceServices(this IServiceCollection services)
        {
            services.AddTransient<IAccountOwnerRepository, AccountOwnerRepository>();
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();

            return services;
        }
    }
}
