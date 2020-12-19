using System;
using System.Threading;

namespace Smoke.Tests.Helpers
{
    internal class SmokeTestThrowingAnAccessViolationException : ITestWithSmoke
    {
        private readonly TimeSpan _duration;

        public SmokeTestThrowingAnAccessViolationException(TimeSpan duration)
        {
            _duration = duration;
        }

        public SmokeTestResult Execute()
        {
            Thread.Sleep(_duration);
            throw new AccessViolationException("oh la la... ah oui oui");
        }
    }
}