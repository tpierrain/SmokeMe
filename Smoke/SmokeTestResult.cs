using System;

namespace Smoke
{

    /// <summary>
    /// Result of a <see cref="ITestWithSmoke"/> execution.
    /// </summary>
    public class SmokeTestResult
    {

        public bool Outcome { get; }
        public Error ErrorMessage { get; }

        public SmokeTestResult(string errorMessage, Exception exception)
        {
            Outcome = false;
            ErrorMessage = new Error(errorMessage, exception);
        }

        public SmokeTestResult(bool? outcome = null)
        {
            outcome ??= true;
            Outcome = outcome.Value;
        }

        public override string ToString()
        {
            return $"Outcome:{Outcome}";
        }
    }
}