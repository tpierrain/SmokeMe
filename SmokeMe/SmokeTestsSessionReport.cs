using System;
using System.Linq;

namespace SmokeMe
{
    /// <summary>
    /// Result of a smoke test session.
    /// </summary>
    public class SmokeTestsSessionReport
    {
        /// <summary>
        /// Gets all the <see cref="SmokeTestResultWithMetaData"/> results of this Smoke test session.
        /// </summary>
        public SmokeTestResultWithMetaData[] Results { get; }

        /// <summary>
        /// Returns <b>true</b> if the Smoke test session is succeeded (i.e. all smoke test succeeded), <b>false</b> otherwise.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Null object for by default initializations.
        /// </summary>
        public static SmokeTestsSessionReport Null => new SmokeTestsSessionReport(new SmokeTestResultWithMetaData[0], false);

        /// <summary>
        /// Gets the status of the smoke tests session report.
        /// </summary>
        public string Status { get; }

        /// <summary>
        /// Instantiates a <see cref="SmokeTestsSessionReport"/>.
        /// </summary>
        /// <param name="results">The results of this Smoke test session.</param>
        /// <param name="isSuccess">Whether or not the <see cref="SmokeTestsSessionReport"/> is successful or not.</param>
        /// <param name="status">The status of the report.</param>
        public SmokeTestsSessionReport(SmokeTestResultWithMetaData[] results, bool? isSuccess = null, string status = null)
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

            status ??= string.Empty;
            Status = status;
        }
    }
}