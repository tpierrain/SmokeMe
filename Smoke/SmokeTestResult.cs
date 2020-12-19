using System;

namespace Smoke
{

    /// <summary>
    /// Result of a <see cref="ISmokeTestAScenario"/> execution.
    /// </summary>
    public class SmokeTestResult
    {

        /// <summary>
        /// Indicates whether the outcome of this <see cref="ISmokeTestAScenario"/> execution is positive or not.
        /// </summary>
        public bool Outcome { get; }

        /// <summary>
        /// Gets the <see cref="Error"/> associated to this <see cref="ISmokeTestAScenario"/> execution.
        /// </summary>
        public Error ErrorMessage { get; }

        /// <summary>
        /// Instantiates a <see cref="SmokeTestResult"/>.
        /// </summary>
        /// <param name="errorMessage">The error message associated to the smoke test execution.</param>
        /// <param name="exception">The <see cref="Exception"/> associated to the smoke test execution.</param>
        public SmokeTestResult(string errorMessage, Exception exception)
        {
            Outcome = false;
            ErrorMessage = new Error(errorMessage, exception);
        }

        /// <summary>
        /// Instantiates a <see cref="SmokeTestResult"/>.
        /// </summary>
        /// <param name="outcome">The outcome of this <see cref="ISmokeTestAScenario"/> execution.</param>
        public SmokeTestResult(bool? outcome = null)
        {
            outcome ??= true;
            Outcome = outcome.Value;
        }

        /// <summary>
        /// Returns a string representing the object.
        /// </summary>
        /// <returns>A string representing the object.</returns>
        public override string ToString()
        {
            return $"Outcome:{Outcome}";
        }
    }
}