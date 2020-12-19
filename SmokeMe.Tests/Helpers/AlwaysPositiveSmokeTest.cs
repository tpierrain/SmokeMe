using System;
using System.Threading;

namespace SmokeMe.Tests.Helpers
{
    internal class AlwaysPositiveSmokeTest : ISmokeTestAScenario
    {
        private readonly TimeSpan _duration;

        public AlwaysPositiveSmokeTest(TimeSpan duration)
        {
            _duration = duration;
        }

        public SmokeTestResult ExecuteScenario()
        {
            Thread.Sleep(_duration);

            return new SmokeTestResult(true);
        }
    }
}