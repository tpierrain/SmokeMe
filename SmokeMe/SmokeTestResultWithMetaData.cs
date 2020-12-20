using System;

namespace SmokeMe
{
    /// <summary>
    /// Result of a <see cref="ICheckSmoke"/> execution enhanced with meta data about it and its execution (like the <see cref="Duration"/>).
    /// </summary>
    public class SmokeTestResultWithMetaData
    {
        /// <summary>
        /// Indicates whether the outcome of this <see cref="ICheckSmoke"/> execution is positive or not.
        /// </summary>
        public bool Outcome => SmokeTestResult.Outcome;
        
        /// <summary>
        /// Gets the <see cref="Error"/> associated to this <see cref="ICheckSmoke"/> execution.
        /// </summary>
        public Error ErrorMessage => SmokeTestResult.ErrorMessage;

        /// <summary>
        /// Gets the duration of this <see cref="ICheckSmoke"/> execution.
        /// </summary>
        public TimeSpan Duration { get; }

        /// <summary>
        /// Gets the name of the executed <see cref="ICheckSmoke"/> instance.
        /// </summary>
        public string SmokeTestName { get; }

        /// <summary>
        /// Gets the description of the executed <see cref="ICheckSmoke"/> instance.
        /// </summary>
        public string SmokeTestDescription { get; }

        private SmokeTestResult SmokeTestResult { get; }
        
        /// <summary>
        /// Instantiates a <see cref="SmokeTestResultWithMetaData"/>.
        /// </summary>
        /// <param name="smokeTestResult">The <see cref="SmokeTestResult"/> associated with this <see cref="ICheckSmoke"/> execution.</param>
        /// <param name="duration">The duration of this <see cref="ICheckSmoke"/> execution.</param>
        public SmokeTestResultWithMetaData(SmokeTestResult smokeTestResult, TimeSpan duration, string smokeTestName, string smokeTestDescription)
        {
            SmokeTestResult = smokeTestResult;
            Duration = duration;
            SmokeTestName = smokeTestName;
            SmokeTestDescription = smokeTestDescription;
        }

        /// <summary>
        /// Returns a string representing the object.
        /// </summary>
        /// <returns>A string representing the object.</returns>
        public override string ToString()
        {
            return $"Outcome:{SmokeTestResult.Outcome}({Duration.TotalMilliseconds} msec)";
        }
    }
}