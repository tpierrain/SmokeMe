using System;
using Microsoft.Extensions.Configuration;

namespace SmokeMe.Helpers
{
    /// <summary>
    /// Extension methods for <see cref="IConfiguration"/>.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Gets the Global timeout value used by the SmokeMe library (default value may be overriden through configuration file).
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance used by the API.</param>
        /// <returns>The Global timeout value.</returns>
        public static TimeSpan GetSmokeMeGlobalTimeout(this IConfiguration configuration)
        {
            var globalTimeout = TimeSpan.FromMilliseconds(Constants.GlobalTimeoutInMsecDefaultValue); // default value
            if (int.TryParse(configuration[Constants.GlobaltimeoutinmsecConfigurationKey], out var globalTimeoutInMsec))
            {
                globalTimeout = TimeSpan.FromMilliseconds(globalTimeoutInMsec); // overriden by the one in the configuration (if valid)
            }

            return globalTimeout;
        }

        /// <summary>
        /// Gets an indication whether the smoke tests execution is enabled or not (default <b>true</b> value may be overriden through configuration file).
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance used by the API.</param>
        /// <returns><b>true</b> if the smoke test execution is enabled or not, <b>false</b> otherwise.</returns>
        public static bool IsSmokeTestExecutionEnabled(this IConfiguration configuration)
        {
            if (!bool.TryParse(configuration[Constants.IsEnabledConfigurationKey], out var isEnabled))
            {
                isEnabled = true;
            }

            return isEnabled;
        }
    }
}