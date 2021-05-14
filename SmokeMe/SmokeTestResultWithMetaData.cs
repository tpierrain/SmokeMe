using System;

namespace SmokeMe
{
    /// <summary>
    /// Result of a <see cref="SmokeTest"/> execution enhanced with meta data about it and its execution (like the <see cref="Duration"/>).
    /// </summary>
    public class SmokeTestResultWithMetaData
    {
        /// <summary>
        /// Indicates whether the outcome of this <see cref="SmokeTest"/> execution is positive or not.
        /// </summary>
        public bool Outcome => SmokeTestResult.Outcome;
        
        /// <summary>
        /// Gets the <see cref="Error"/> associated to this <see cref="SmokeTest"/> execution.
        /// </summary>
        public Error ErrorMessage => SmokeTestResult.ErrorMessage;

        /// <summary>
        /// Gets the duration of this <see cref="SmokeTest"/> execution.
        /// </summary>
        public TimeSpan? Duration { get; }

        /// <summary>
        /// Gets the name of the executed <see cref="SmokeTest"/> instance.
        /// </summary>
        public string SmokeTestName { get; }

        /// <summary>
        /// Gets the description of the executed <see cref="SmokeTest"/> instance.
        /// </summary>
        public string SmokeTestDescription { get; }

        private SmokeTestResult SmokeTestResult { get; }

        public string[] SmokeTestCategories { get; }

        public Status Status { get; } = Status.Executed;
        
        public string SmokeTestType { get; }

        /// <summary>
        /// Instantiates a <see cref="SmokeTestResultWithMetaData"/>.
        /// </summary>
        /// <param name="smokeTestResult">The <see cref="SmokeTestResult"/> associated with this <see cref="SmokeTest"/> execution.</param>
        /// <param name="duration">The duration of this <see cref="SmokeTest"/> execution.</param>
        /// <param name="smokeTestName">Name of the smoke test.</param>
        /// <param name="smokeTestDescription">Description of the smoke test.</param>
        /// <param name="smokeTestCategories"></param>
        /// <param name="discarded"></param>
        private SmokeTestResultWithMetaData(SmokeTestResult smokeTestResult, TimeSpan? duration, string smokeTestName, string smokeTestDescription,
            string[] smokeTestCategories, bool? discarded, string smokeTestType)
        {
            SmokeTestResult = smokeTestResult;
            Duration = duration;
            SmokeTestName = smokeTestName;
            SmokeTestDescription = smokeTestDescription;
            SmokeTestCategories = smokeTestCategories;
            SmokeTestType = smokeTestType;

            if (discarded.HasValue && discarded.Value == true)
            {
                Status = Status.Discarded;
            }
            else
            {
                Status = Status.Executed;
            }
        }

        public SmokeTestResultWithMetaData(SmokeTestResult smokeTestResult, TimeSpan? duration, SmokeTestInstanceWithMetaData smokeTestInstanceWithMetaData, string smokeTestType,
            bool? discarded = null) : this(smokeTestResult, duration, smokeTestInstanceWithMetaData.SmokeTest.SmokeTestName, smokeTestInstanceWithMetaData.SmokeTest.Description, smokeTestInstanceWithMetaData.Categories, discarded, smokeTestType)
        {
        }

        /// <summary>
        /// Returns a string representing the object.
        /// </summary>
        /// <returns>A string representing the object.</returns>
        public override string ToString()
        {
            if (Duration.HasValue)
            {
                return $"Outcome:{SmokeTestResult.Outcome}({Duration.Value.TotalMilliseconds} msec)";
            }
            return $"Outcome:{SmokeTestResult.Outcome}(no defined Duration)";
        }
    }
}