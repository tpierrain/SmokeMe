using System;

namespace SmokeMe
{
    /// <summary>
    /// Configuration abstraction for the SmokeMe library.
    /// Decoupled from ASP.NET Core's IConfiguration to allow usage in non-ASP.NET contexts.
    /// </summary>
    public interface ISmokeTestConfiguration
    {
        /// <summary>
        /// Gets the global timeout for all smoke tests execution.
        /// </summary>
        TimeSpan GlobalTimeout { get; }

        /// <summary>
        /// Gets a value indicating whether the smoke test execution is enabled.
        /// </summary>
        bool IsExecutionEnabled { get; }
    }
}
