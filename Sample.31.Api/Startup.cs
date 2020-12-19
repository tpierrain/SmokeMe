using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Diverse;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sample.Api.FakeDomain;
using Sample.ExternalSmokeTests.Utilities;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Sample.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddMvcOptions((Action<MvcOptions>)(o => o.EnableEndpointRouting = false));

            services.AddControllers()
                .AddNewtonsoftJson((Action<MvcNewtonsoftJsonOptions>)(options => options.SerializerSettings.Converters.Add((JsonConverter)new StringEnumConverter())));

            services.AddVersioning();

            Fuzzer.Log +=obj =>
            {
            };

            // -------- Specific services for the API (no need to register anything for Smoke lib usage ---------
            services.AddSingleton<IRestClient, RestClient>();

            services.AddTransient<IFuzz, Fuzzer>();
            services.AddTransient<IProviderNumbers, NumberProvider>();
            // --------------------------------------------------------------------------------------------------

            services.AddSwaggerGenNewtonsoftSupport();

            var apiContact = new OpenApiContact()
            {
                Name = "Super contact team",
                Email = "api.me@whatever.com"
            };

            var swaggerTitle = this.GetType().Assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "";

            services.AddSwaggerGen((Action<SwaggerGenOptions>)(o =>
            {
                var str = Path.Combine(AppContext.BaseDirectory, Assembly.GetExecutingAssembly().GetName().Name ?? string.Empty) + ".xml";
                if (!File.Exists(str))
                    return;
                o.IncludeXmlComments(str);
            }));

            services.AddSwaggerGeneration(apiContact, swaggerTitle, this.GetType());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ConfigureSwagger(app, provider);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// Override this method if you need to configure Swagger endpoint, e.g. using UseSwagger() and UseSwaggerUI() extension methods.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="provider"></param>
        protected virtual void ConfigureSwagger(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI((Action<SwaggerUIOptions>)(options =>
            {
                var swaggerUiOptions = options;
                var entryAssembly = Assembly.GetEntryAssembly();
                var str = ((object)entryAssembly != null ? entryAssembly.GetName().Name : (string)null) + " - Swagger";
                swaggerUiOptions.DocumentTitle = str;
                foreach (var versionDescription in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint("/swagger/" + versionDescription.GroupName + "/swagger.json", versionDescription.GroupName.ToUpperInvariant());
                }
            }));
        }

    }

    /// <summary>
    /// Extension methods for <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> instances.
    /// </summary>
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSwaggerGeneration(
            this IServiceCollection services,
            OpenApiContact apiContact,
            string swaggerTitle,
            Type callerType)
        {
            return services.AddSwaggerGen((Action<SwaggerGenOptions>)(options =>
            {
                foreach (ApiVersionDescription versionDescription in (IEnumerable<ApiVersionDescription>)services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>().ApiVersionDescriptions)
                    options.SwaggerDoc(versionDescription.GroupName, new OpenApiInfo()
                    {
                        Title = swaggerTitle + $" {(object)versionDescription.ApiVersion}",
                        Version = versionDescription.ApiVersion.ToString(),
                        Contact = apiContact
                    });
                options.OperationFilter<SwaggerDefaultValues>();
                options.IncludeXmlComments(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), callerType.Assembly.GetName().Name + ".xml"));
            }));
        }

        public static IServiceCollection AddVersioning(this IServiceCollection services)
        {
            return services.AddVersionedApiExplorer((Action<ApiExplorerOptions>)(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            })).AddApiVersioning((Action<ApiVersioningOptions>)(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(new DateTime(2016, 7, 1));
            }));
        }
    }

    /// <summary>
    /// Represents the Swagger/Swashbuckle operation filter used to document the implicit API version parameter.
    /// </summary>
    /// <remarks>This <see cref="T:Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter" /> is only required due to bugs in the <see cref="T:Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator" />.
    /// Once they are fixed and published, this class can be removed.</remarks>
    public class SwaggerDefaultValues : IOperationFilter
    {
        /// <summary>
        /// Applies the filter to the specified operation using the given context.
        /// </summary>
        /// <param name="operation">The operation to apply the filter to.</param>
        /// <param name="context">The current operation filter context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                return;
            }

            foreach (var parameter1 in (IEnumerable<OpenApiParameter>)operation.Parameters)
            {
                var parameter = parameter1;
                var parameterDescription = context.ApiDescription.ParameterDescriptions.First<ApiParameterDescription>((Func<ApiParameterDescription, bool>)(p => p.Name == parameter.Name));
                var routeInfo = parameterDescription.RouteInfo;
                parameter.Description ??= parameterDescription.ModelMetadata?.Description;

                if (routeInfo != null)
                {
                    parameter.Required |= !routeInfo.IsOptional;
                }
            }
        }
    }


}
