using System;
using System.IO;
using Appical.Domain.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Appical.Api.Helper
{
    public static class SwaggerHelper
    {
        public static void SetUpSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(ApiAccess.All, CreateInfoForApiVersion(ApiAccess.All));
                c.SwaggerDoc(ApiAccess.BankClerk, CreateInfoForApiVersion(ApiAccess.BankClerk));
                c.SwaggerDoc(ApiAccess.AccountOwner, CreateInfoForApiVersion(ApiAccess.AccountOwner));
                c.DocInclusionPredicate(DocInclusionPredicate);

                // Set the comments path for the Swagger JSON and UI.
                const string xmlFile = "Appical.Api.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        public static void ConfigureSwagger(SwaggerUIOptions swaggerOptions)
        {
            swaggerOptions.SwaggerEndpoint($"/swagger/{ApiAccess.All}/swagger.json", "Appical API - All");
            swaggerOptions.SwaggerEndpoint($"/swagger/{ApiAccess.BankClerk}/swagger.json", "Appical API - Bank Clerk operations");
            swaggerOptions.SwaggerEndpoint($"/swagger/{ApiAccess.AccountOwner}/swagger.json", "Appical API - Account Owner operations");
            swaggerOptions.DocumentTitle = "Appical API Documentation";
            swaggerOptions.RoutePrefix = string.Empty;
            swaggerOptions.DisplayRequestDuration();
        }

        /// <summary>
        /// Determines which controllers and methods are visible in the Swagger Docs, right now everything is permitted
        /// </summary>
        /// <param name="version"></param>
        /// <param name="apiDescription"></param>
        /// <returns></returns>
        public static bool DocInclusionPredicate(string version, ApiDescription apiDescription)
        {
            dynamic controllerName = (apiDescription.ActionDescriptor as dynamic)?.ControllerName ?? string.Empty;
            if (string.IsNullOrEmpty(controllerName)) return false;

            if (version.Equals(ApiAccess.All)) return true;

            if (version.Equals(ApiAccess.BankClerk)) return ApiAccess.BankClerkControllers.Contains(controllerName);
            if (version.Equals(ApiAccess.AccountOwner)) return ApiAccess.AccountOwnerControllers.Contains(controllerName);

            return false;
        }

        /// <summary>
        /// General information about the Swagger API Documentation
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static OpenApiInfo CreateInfoForApiVersion(string version)
        {
            OpenApiInfo info = new OpenApiInfo
            {
                Version = version,
                Title = "Appical Bank Account Management API",
                Description = "Documentation of the Appical Bank Account Management API. Technical Assessment project",
                Contact = new OpenApiContact
                {
                    Name = "Kurt Lourens",
                    Email = "hi@kurtlourens.com",
                    Url = new Uri("https://kurtlourens.com")
                }
            };
            return info;
        }
    }
}
