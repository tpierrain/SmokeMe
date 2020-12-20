using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmokeMe.Tests.Helpers
{
    internal class SmokeTestThrowingAnAccessViolationException : ISmokeTestAScenario
    {
        private readonly TimeSpan _delay;

        public string SmokeTestName => "Throwing exception after a delay";
        public string Description => $"For unit testing purpose. Throws an exception after a delay of {_delay.TotalMilliseconds} milliseconds.";

        public SmokeTestThrowingAnAccessViolationException(TimeSpan delay)
        {
            _delay = delay;
        }

        public Task<SmokeTestResult> ExecuteScenario()
        {
            Thread.Sleep(_delay);
            throw new AccessViolationException("oh la la... ah oui oui");
        }
    }
}