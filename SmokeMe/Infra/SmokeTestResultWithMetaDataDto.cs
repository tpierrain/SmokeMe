using System;

namespace SmokeMe.Infra
{
    /// <summary>
    /// Result of a <see cref="SmokeTest"/> execution enhanced with meta data about it and its execution (like the <see cref="Duration"/>).
    /// </summary>
    public class SmokeTestResultWithMetaDataDto
    {
        /// <summary>
        /// The name of the <see cref="SmokeTest"/> smoke test (as declared by it).
        /// </summary>
        public string SmokeTestName { get; }

        /// <summary>
        /// The description of what this <see cref="SmokeTest"/> smoke test is about (as declared by it).
        /// </summary>
        public string SmokeTestDescription { get; }

        /// <summary>
        /// Gets the list of <see cref="SmokeTestCategories"/> attributes related to this Smoke test (i.e.: <see cref="SmokeTest"/>).
        /// </summary>
        public string SmokeTestCategories { get; }

        /// <summary>
        /// Indicates whether the outcome of this <see cref="SmokeTest"/> execution is positive or not.
        /// </summary>
        public bool Outcome { get; }

        /// <summary>
        /// Gets the <see cref="Error"/> associated to this <see cref="SmokeTest"/> execution.
        /// </summary>
        public Error Error { get; }

        /// <summary>
        /// Gets the human-readable duration of this smoke test execution.
        /// </summary>
        public string Duration { get; }

        /// <summary>
        /// Gets the duration in milliseconds of this smoke test execution (useful for scripting).
        /// </summary>
        public double? DurationInMsec { get; }

        public Status Status { get; }

        public string SmokeTestType { get; }

        /// <summary>
        /// Instantiates a <see cref="SmokeTestResultWithMetaDataDto"/>.
        /// </summary>
        /// <param name="smokeTestName"></param>
        /// <param name="smokeTestDescription"></param>
        /// <param name="outcome"></param>
        /// <param name="error"></param>
        /// <param name="durationTimespan"></param>
        /// <param name="duration"></param>
        /// <param name="status"></param>
        /// <param name="smokeTestCategories"></param>
        /// <param name="argSmokeTestCategories"></param>
        public SmokeTestResultWithMetaDataDto(string smokeTestName, string smokeTestDescription, bool outcome, Error error, TimeSpan? durationTimespan,
            string duration, Status status, string[] smokeTestCategories, string smokeTestType)
        {
            SmokeTestName = smokeTestName;
            SmokeTestDescription = smokeTestDescription;
            SmokeTestType = smokeTestType;
            Outcome = outcome;
            Error = error;
            
            duration ??= "timeout";

            Duration = duration;
            Status = status;
            SmokeTestCategories = string.Join(", ", smokeTestCategories);

            if (durationTimespan.HasValue)
            {
                DurationInMsec = durationTimespan.Value.TotalMilliseconds;
            }
        }
    }
}