using System;
using Microsoft.Extensions.Configuration;

namespace SmokeMe.AspNetCore
{
    /// <summary>
    /// Adapts ASP.NET Core's <see cref="IConfiguration"/> to <see cref="ISmokeTestConfiguration"/>.
    /// Reads values from the "Smoke:" configuration section.
    /// </summary>
    public class SmokeMeConfigurationAdapter : ISmokeTestConfiguration
    {
        private readonly IConfiguration _configuration;
        private readonly SmokeMeOptions _defaults;

        /// <summary>
        /// Instantiates a <see cref="SmokeMeConfigurationAdapter"/>.
        /// </summary>
        /// <param name="configuration">The ASP.NET Core configuration.</param>
        /// <param name="defaults">Default options (may be overridden by configuration values).</param>
        public SmokeMeConfigurationAdapter(IConfiguration configuration, SmokeMeOptions defaults = null)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _defaults = defaults ?? new SmokeMeOptions();
        }

        /// <inheritdoc />
        public TimeSpan GlobalTimeout
        {
            get
            {
                if (int.TryParse(_configuration[Constants.GlobaltimeoutinmsecConfigurationKey], out var globalTimeoutInMsec))
                {
                    return TimeSpan.FromMilliseconds(globalTimeoutInMsec);
                }

                return _defaults.GlobalTimeout;
            }
        }

        /// <inheritdoc />
        public bool IsExecutionEnabled
        {
            get
            {
                if (bool.TryParse(_configuration[Constants.IsEnabledConfigurationKey], out var isEnabled))
                {
                    return isEnabled;
                }

                return _defaults.IsExecutionEnabled;
            }
        }
    }
}
