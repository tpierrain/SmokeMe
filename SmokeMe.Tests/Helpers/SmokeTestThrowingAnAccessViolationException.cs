using System;
using System.Threading.Tasks;

namespace SmokeMe.Tests.Helpers
{
    internal class SmokeTestThrowingAnAccessViolationException : SmokeTest
    {
        private readonly TimeSpan _delay;

        public override string SmokeTestName => "Throwing exception after a delay";
        public override string Description => $"For unit testing purpose. Throws an exception after a delay of {_delay.TotalMilliseconds} milliseconds.";

        /// <summary>
        /// Gets a value indicating whether or not this smoke test must be discarded (may be interesting to coupled with feature toggle mechanism).
        /// </summary>
        public bool MustBeDiscarded => false;

        public SmokeTestThrowingAnAccessViolationException(TimeSpan delay)
        {
            _delay = delay;
        }

        public override Task<SmokeTestResult> Scenario()
        {
            var continuationTask = Task.Delay(_delay)
                .ContinueWith(task => throw new AccessViolationException("oh la la... ah oui oui"))
                .ContinueWith(task => new SmokeTestResult(false));

            return continuationTask;
        }
    }
}