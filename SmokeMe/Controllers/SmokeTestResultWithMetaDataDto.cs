using System;

namespace SmokeMe.Controllers
{
    /// <summary>
    /// Result of a <see cref="ICheckSmoke"/> execution enhanced with meta data about it and its execution (like the <see cref="Duration"/>).
    /// </summary>
    public class SmokeTestResultWithMetaDataDto
    {
        public string SmokeTestName { get; }
        public string SmokeTestDescription { get; }
        public bool Outcome { get; }
        public Error Error { get; }
        public string Duration { get; }
        public double DurationInMsec { get; }

        public SmokeTestResultWithMetaDataDto(string smokeTestName, string smokeTestDescription, bool outcome, Error error, TimeSpan durationTimespan, string duration)
        {
            SmokeTestName = smokeTestName;
            SmokeTestDescription = smokeTestDescription;
            Outcome = outcome;
            Error = error;
            Duration = duration;
            DurationInMsec = durationTimespan.TotalMilliseconds;
        }
    }
}