using System.Threading.Tasks;

namespace SmokeMe.Tests
{
    [Category("FailingSaMere")]
    public class AlwaysFailingSmokeTest : ICheckSmoke
    {
        public string SmokeTestName => "Failing on purpose";
        public string Description => "Failing Smoke test for testing purpose";

        /// <summary>
        /// Gets a value indicating whether or not this smoke test must be discarded (may be interesting to coupled with feature toggle mechanism).
        /// </summary>
        public bool MustBeDiscarded => false;

        public Task<SmokeTestResult> Scenario()
        {
            throw new System.NotImplementedException();
        }
    }
}