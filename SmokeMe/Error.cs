using System;

namespace SmokeMe
{
    /// <summary>
    /// Error during a <see cref="ISmokeTestAScenario"/> excecution.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Instantiates an <see cref="Error"/>.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="exception">An exception that has been catched during the <see cref="ISmokeTestAScenario"/> execution.</param>
        public Error(string errorMessage, Exception exception)
        {
            Message = errorMessage;
            Exception = exception;
        }

        /// <summary>
        /// The error message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The exception that has been catched during the <see cref="ISmokeTestAScenario"/> execution.
        /// </summary>
        public Exception Exception { get; private set; }
    }
}