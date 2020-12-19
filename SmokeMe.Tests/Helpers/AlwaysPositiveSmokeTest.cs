using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmokeMe.Tests.Helpers
{
    internal class AlwaysPositiveSmokeTest : ISmokeTestAScenario
    {
        private readonly TimeSpan _duration;

        public AlwaysPositiveSmokeTest(TimeSpan duration)
        {
            _duration = duration;
        }

        public Task<SmokeTestResult> ExecuteScenario()
        {
            Thread.Sleep(_duration);

            return Task.FromResult(new SmokeTestResult(true));
        }
    }
}