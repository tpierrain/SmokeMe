using System;
using System.Linq;

namespace SmokeMe
{
    /// <summary>
    /// Result of a smoke test session.
    /// </summary>
    public class SmokeTestSessionResult
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
        /// Instantiates a <see cref="SmokeTestSessionResult"/>.
        /// </summary>
        /// <param name="results">The results of this Smoke test session.</param>
        /// <param name="isSuccess">Whether or not the <see cref="SmokeTestSessionResult"/> is successful or not.</param>
        public SmokeTestSessionResult(SmokeTestResultWithMetaData[] results, bool? isSuccess = null)
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
    }
}