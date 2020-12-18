using System;

namespace Smoke
{

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

    public class StopWatchedSmokeTestExecution
    {
        public bool Outcome => SmokeTestResult.Outcome;
        public Error ErrorMessage => SmokeTestResult.ErrorMessage;
        
        private SmokeTestResult SmokeTestResult { get; }
        public TimeSpan Duration { get; }

        public StopWatchedSmokeTestExecution(SmokeTestResult smokeTestResult, TimeSpan duration)
        {
            SmokeTestResult = smokeTestResult;
            Duration = duration;
        }

        public override string ToString()
        {
            return $"Outcome:{SmokeTestResult.Outcome}({Duration.TotalMilliseconds} msec)";
        }
    }
}