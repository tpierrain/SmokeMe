using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmokeMe.Tests.Helpers
{
    internal class AlwaysPositiveSmokeTest : ISmokeTestAScenario
    {
        private readonly TimeSpan _delay;

        public string SmokeTestName => "Always positive smoke test after a delay";
        public string Description => $"For unit testing purpose. Return positively after a delay of {_delay.TotalMilliseconds} milliseconds";

        public AlwaysPositiveSmokeTest(TimeSpan delay)
        {
            _delay = delay;
        }

        public Task<SmokeTestResult> ExecuteScenario()
        {
            Thread.Sleep(_delay);

            return Task.FromResult(new SmokeTestResult(true));
        }
    }
}