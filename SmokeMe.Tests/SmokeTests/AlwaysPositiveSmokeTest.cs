using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmokeMe.Tests.SmokeTests
{
    [Category("Tests")]
    internal class AlwaysPositiveSmokeTest : SmokeTest
    {
        private readonly TimeSpan _delay;

        public override string SmokeTestName => "Always positive smoke test after a delay";
        public override string Description => $"For unit testing purpose. Return positively after a delay of {_delay.TotalMilliseconds} milliseconds";
        
        public AlwaysPositiveSmokeTest(TimeSpan delay)
        {
            _delay = delay;
        }

        public override Task<SmokeTestResult> Scenario()
        {
            Thread.Sleep(Convert.ToInt32(_delay.TotalMilliseconds));
            return Task.FromResult(new SmokeTestResult(true));

            // The TPL version relies too much on the overall health/availability of the CI agent running the tests.
            //var continuationTask = Task.Delay(_delay)
            //    .ContinueWith(task => new SmokeTestResult(true));
            //return continuationTask;
        }
    }
}