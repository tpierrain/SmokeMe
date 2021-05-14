using System.Threading.Tasks;
using SmokeMe;

namespace Sample.ExternalSmokeTests
{
    [Category("Awkward")]
    [Ignore]
    public class AlwaysWorkingButIgnoredDBSmokeTest : ICheckSmoke
    {
        public string SmokeTestName => "Always working but ignored DB smoke test";
        public string Description => "Succeeding Smoke test for testing purpose";

        /// <summary>
        /// Gets a value indicating whether or not this smoke test must be discarded (may be interesting to coupled with feature toggle mechanism).
        /// </summary>
        public bool MustBeDiscarded => false;

        public Task<SmokeTestResult> Scenario()
        {
            return Task.FromResult(new SmokeTestResult(true));
        }
    }
}