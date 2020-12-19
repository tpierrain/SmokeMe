using System;
using System.Threading;

namespace Smoke.Tests.Helpers
{
    internal class AlwaysPositiveSmokeTest : ITestWithSmoke
    {
        private readonly TimeSpan _duration;

        public AlwaysPositiveSmokeTest(TimeSpan duration)
        {
            _duration = duration;
        }

        public SmokeTestResult Run()
        {
            Thread.Sleep(_duration);

            return new SmokeTestResult(true);
        }
    }
}