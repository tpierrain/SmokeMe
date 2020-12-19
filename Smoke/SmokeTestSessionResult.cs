using System;
using System.Linq;

namespace Smoke
{
    /// <summary>
    /// Result of a smoke test session.
    /// </summary>
    public class SmokeTestSessionResult
    {
        /// <summary>
        /// Gets all the <see cref="StopWatchedSmokeTestExecution"/> results of this Smoke test session.
        /// </summary>
        public StopWatchedSmokeTestExecution[] Results { get; }

        /// <summary>
        /// Returns <b>true</b> if the Smoke test session is succeeded (i.e. all smoke test succeeded), <b>false</b> otherwise.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Instantiates a <see cref="SmokeTestSessionResult"/>.
        /// </summary>
        /// <param name="results">The results of this Smoke test session.</param>
        /// <param name="isSuccess">Whether or not the <see cref="SmokeTestSessionResult"/> is successful or not.</param>
        public SmokeTestSessionResult(StopWatchedSmokeTestExecution[] results, bool? isSuccess = null)
        {
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            if (isSuccess != null && isSuccess.Value == false)
            {
                IsSuccess = false;
            }
            else
            {
                IsSuccess = (results.Length > 0) && (results.All(x => x.Outcome != false));
            }
            
            Results = results;
        }

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
        /// Gets the Azure region name where this API instance is running.
        /// </summary>
        public string AzureRegionName { get; }
        
        /// <summary>
        /// Gets the number of Processors this API instance has.
        /// </summary>
        public string NbOfProcessors { get; }
    }
}