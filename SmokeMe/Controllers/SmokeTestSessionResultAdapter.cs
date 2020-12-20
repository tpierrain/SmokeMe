using System;
using System.Globalization;
using System.Linq;

namespace SmokeMe.Controllers
{
    /// <summary>
    /// Adapter from SmokeMe internal model to SmokeMe external DTOs.
    /// </summary>
    public static class SmokeTestSessionResultAdapter
    {
        /// <summary>
        /// Adapts a <see cref="SmokeTestSessionResult"/> instance to a <see cref="SmokeTestSessionResultDto"/> one.
        /// </summary>
        /// <param name="results">The <see cref="SmokeTestSessionResult"/> instance to adapt.</param>
        /// <param name="runtimeDescription">The <see cref="ApiRuntimeDescription"/> to associate</param>
        /// <returns>The <see cref="SmokeTestSessionResultDto"/> corresponding to the external exposition model of the provided <see cref="SmokeTestSessionResult"/> instance.</returns>
        public static SmokeTestSessionResultDto Adapt(SmokeTestSessionResult results, ApiRuntimeDescription runtimeDescription)
        {
            // Adapt the array of results 
            var resultsDto = results.Results
                .Select(r => new SmokeTestResultWithMetaDataDto(r.SmokeTestName, r.SmokeTestDescription, r.Outcome, r.ErrorMessage, r.Duration, AdaptDurationToMakeItReadable(r.Duration)));

            // Adapt the overall wrapper (with runtime description information too)
            var result = new SmokeTestSessionResultDto(results, runtimeDescription, resultsDto);

            return result;
        }

        /// <summary>
        /// Adapts <see cref="TimeSpan"/> duration to a human readable string version (with "en-us" culture format).
        /// </summary>
        /// <param name="duration">The <see cref="TimeSpan"/> to be adapted.</param>
        /// <returns>The human readable string version (with "en-us" culture format) of the <see cref="TimeSpan"/> provided.</returns>
        public static string AdaptDurationToMakeItReadable(TimeSpan duration)
        {
            var usCultureInfo = CultureInfo.GetCultureInfo("en-us");
            if (duration < TimeSpan.FromSeconds(1))
            {
                var roundedMilliseconds = Convert.ToInt32(duration.TotalMilliseconds);
                if (roundedMilliseconds <= 1)
                {
                    return $"{roundedMilliseconds.ToString(usCultureInfo)} millisecond";
                }

                return $"{roundedMilliseconds.ToString(usCultureInfo)} milliseconds";
            }

            if (duration == TimeSpan.FromSeconds(1))
            {
                return $"1 second";
            }

            if (duration < TimeSpan.FromMinutes(1))
            {
                if (duration.TotalSeconds < 1.1)
                {
                    return "1 second";
                }

                var roundedSeconds = Math.Round(duration.TotalSeconds, 1);
                return $"{roundedSeconds.ToString(usCultureInfo)} seconds";
            }

            return duration.ToString("c");
        }
    }
}