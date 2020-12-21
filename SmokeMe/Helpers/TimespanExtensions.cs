using System;
using System.Globalization;

namespace SmokeMe.Helpers
{
    /// <summary>
    /// Extension methods for <see cref="TimeSpan"/>.
    /// </summary>
    public static class TimespanExtensions
    {
        /// <summary>
        /// Adapts <see cref="TimeSpan"/> duration to a human readable string version (with "en-us" culture format).
        /// </summary>
        /// <param name="duration">The <see cref="TimeSpan"/> to be adapted.</param>
        /// <returns>The human readable string version (with "en-us" culture format) of the <see cref="TimeSpan"/> provided.</returns>
        public static string GetHumanReadableVersion(this TimeSpan duration)
        {
            var usCultureInfo = CultureInfo.GetCultureInfo("en-us");

            if (duration < TimeSpan.FromMilliseconds(1))
            {
                var totalDurationInMicroseconds = duration.TotalMilliseconds*1000;
                var roundedMicroseconds = Convert.ToInt32(totalDurationInMicroseconds);
                return $"{roundedMicroseconds.ToString(usCultureInfo)} microseconds";
            }

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