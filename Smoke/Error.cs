using System;

namespace Smoke
{
    /// <summary>
    /// Error during a <see cref="ITestWithSmoke"/> excecution.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Instantiates an <see cref="Error"/>.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="exception">An exception that has been catched during the <see cref="ITestWithSmoke"/> execution.</param>
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
        /// The exception that has been catched during the <see cref="ITestWithSmoke"/> execution.
        /// </summary>
        public Exception Exception { get; private set; }
    }
}