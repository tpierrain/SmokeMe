using System;
using System.Linq;

namespace Smoke
{
    /// <summary>
    /// Result of a smoke test session.
    /// </summary>
    public class SmokeTestSessionResult
    {
        public StopWatchedSmokeTestExecution[] Results { get; }

        public bool IsSuccess { get; }

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

        public string InstanceIdentifier { get; }
        public string ApiVersion { get; }
        public string OsName { get; }
        public string AzureRegionName { get; }
        public string NbOfProcessors { get; }
    }
}