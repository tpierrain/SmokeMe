using System;

namespace SmokeMe
{
    /// <summary>
    /// Options for configuring the SmokeMe library.
    /// </summary>
    public class SmokeMeOptions : ISmokeTestConfiguration
    {
        /// <summary>
        /// Gets or sets the global timeout for all smoke tests execution.
        /// Default: 30 seconds.
        /// </summary>
        public TimeSpan GlobalTimeout { get; set; } = TimeSpan.FromMilliseconds(Constants.GlobalTimeoutInMsecDefaultValue);

        /// <summary>
        /// Gets or sets a value indicating whether the smoke test execution is enabled.
        /// Default: true.
        /// </summary>
        public bool IsExecutionEnabled { get; set; } = true;
    }
}
