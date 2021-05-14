using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmokeMe.Tests.Helpers
{
    [Category("Tests")]
    internal class AlwaysPositiveSmokeTest : ICheckSmoke
    {
        private readonly TimeSpan _delay;

        public string SmokeTestName => "Always positive smoke test after a delay";
        public string Description => $"For unit testing purpose. Return positively after a delay of {_delay.TotalMilliseconds} milliseconds";

        /// <summary>
        /// Gets a value indicating whether or not this smoke test must be discarded (may be interesting to coupled with feature toggle mechanism).
        /// </summary>
        public bool MustBeDiscarded => false;

        public AlwaysPositiveSmokeTest(TimeSpan delay)
        {
            _delay = delay;
        }

        public Task<SmokeTestResult> Scenario()
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