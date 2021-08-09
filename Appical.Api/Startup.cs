using Appical.Api.Helper;
using Appical.Data.Helper;
using Appical.Domain.Configuration;
using Appical.Domain.Configuration.Interface;
using Appical.Persistence;
using Appical.Persistence.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Appical.Api
{
    public class Startup
    {
        public IApiConfiguration ApiConfiguration { get; set; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ApiConfiguration = Configuration.Get<ApiConfiguration>();

            services.RegisterConfiguration(ApiConfiguration);
            services.RegisterPersistenceServices();

            services.SetUpEntityFramework(ApiConfiguration.Database);
            services.SetUpSwagger();

            services.AddRouting();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppicalContext db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            db.Database.Migrate();

            app.UseSwagger();
            app.UseSwaggerUI(SwaggerHelper.ConfigureSwagger);

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
