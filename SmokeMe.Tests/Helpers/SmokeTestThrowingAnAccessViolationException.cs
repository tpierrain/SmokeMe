using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmokeMe.Tests.Helpers
{
    internal class SmokeTestThrowingAnAccessViolationException : ISmokeTestAScenario
    {
        private readonly TimeSpan _duration;

        public SmokeTestThrowingAnAccessViolationException(TimeSpan duration)
        {
            _duration = duration;
        }

        public Task<SmokeTestResult> ExecuteScenario()
        {
            Thread.Sleep(_duration);
            throw new AccessViolationException("oh la la... ah oui oui");
        }
    }
}