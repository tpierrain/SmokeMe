using System;

namespace SmokeMe
{
    /// <summary>
    /// Represents a failed (due to timeout) smoke test session.
    /// </summary>
    public class TimeoutSmokeTestsSessionReport : SmokeTestsSessionReport
    {
        private readonly TimeSpan _globalTimeout;

        /// <summary>
        /// Instantiates a <see cref="TimeoutSmokeTestsSessionReport"/>.
        /// </summary>
        /// <param name="globalTimeout">The global timeout expiration that led to his failure.</param>
        /// <param name="completedResults">The results we could get before the smoke test execution session timeouts.</param>
        /// <param name="status">The status of the report.</param>
        public TimeoutSmokeTestsSessionReport(TimeSpan globalTimeout, SmokeTestResultWithMetaData[] completedResults,  string status) : base(completedResults, false, status)
        {
            _globalTimeout = globalTimeout;
        }

    }
}