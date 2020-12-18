using System;

namespace Smoke
{
    public class Error
    {
        public Error(string errorMessage, Exception exception)
        {
            Message = errorMessage;
            Exception = exception;
        }

        public string Message { get; private set; }
        public Exception Exception { get; private set; }
    }
}