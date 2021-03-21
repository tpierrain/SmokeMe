using System.Threading.Tasks;
using SmokeMe;

namespace Sample.ExternalSmokeTests
{
    [SmokeTestCategory("Awkward")]
    [Ignored]
    public class AlwaysWorkingButIgnoredDBSmokeTest : ICheckSmoke
    {
        public string SmokeTestName => "Always working but ignored DB smoke test";
        public string Description => "Succeeding Smoke test for testing purpose";
        public Task<SmokeTestResult> Scenario()
        {
            return Task.FromResult(new SmokeTestResult(true));
        }
    }
}