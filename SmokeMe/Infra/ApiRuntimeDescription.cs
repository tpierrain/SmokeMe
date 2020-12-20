using System;
using System.Reflection;

namespace SmokeMe.Infra
{
    /// <summary>
    /// A few information about an API and its execution runtime.
    /// </summary>
    public class ApiRuntimeDescription
    {
        /// <summary>
        /// Gets the API instance identifier.
        /// </summary>
        public string InstanceIdentifier { get; }

        /// <summary>
        /// Gets the API version.
        /// </summary>
        public string ApiVersion { get; }

        /// <summary>
        /// Gets the OS name of this API instance.
        /// </summary>
        public string OsName { get; }

        /// <summary>
        /// Gets the region name where this API instance is running (for API running on Azure only).
        /// </summary>
        public string AzureRegionName { get; }

        /// <summary>
        /// Gets the number of Processors this API instance has.
        /// </summary>
        public string NbOfProcessors { get; }

        public ApiRuntimeDescription()
        {
            try
            {
                ApiVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
            catch { }

            OsName = Environment.GetEnvironmentVariable("OS");
            AzureRegionName = Environment.GetEnvironmentVariable("REGION_NAME");
            NbOfProcessors = Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS");
        }
    }
}