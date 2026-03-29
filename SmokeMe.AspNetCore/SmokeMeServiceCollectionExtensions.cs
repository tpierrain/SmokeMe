using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SmokeMe.AspNetCore
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> to register SmokeMe services.
    /// </summary>
    public static class SmokeMeServiceCollectionExtensions
    {
        /// <summary>
        /// Registers SmokeMe services (smoke test discovery, configuration) into the DI container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureOptions">Optional action to configure <see cref="SmokeMeOptions"/>.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddSmokeMe(this IServiceCollection services, Action<SmokeMeOptions> configureOptions = null)
        {
            var options = new SmokeMeOptions();
            configureOptions?.Invoke(options);

            services.TryAddSingleton<ISmokeTestConfiguration>(sp =>
            {
                var configuration = sp.GetService<IConfiguration>();
                if (configuration != null)
                {
                    return new SmokeMeConfigurationAdapter(configuration, options);
                }

                return options;
            });

            services.TryAddSingleton<IFindSmokeTests>(sp => new SmokeTestAutoFinder(sp));

            return services;
        }
    }
}
