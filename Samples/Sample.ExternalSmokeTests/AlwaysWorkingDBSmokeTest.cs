using System.Threading.Tasks;
using SmokeMe;

namespace Sample.ExternalSmokeTests
{
    [SmokeTestCategory("DB")]
    [SmokeTestCategory("Booking")]
    public class AlwaysWorkingDBSmokeTest : ICheckSmoke
    {
        public string SmokeTestName => "Always working DB smoke test";
        public string Description => "Succeeding Smoke test for testing purpose";
        public Task<SmokeTestResult> Scenario()
        {
            return Task.FromResult(new SmokeTestResult(true));
        }
    }
}