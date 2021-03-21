using System.Threading.Tasks;
using SmokeMe;

namespace Sample.ExternalSmokeTests
{
    [Category("DB")]
    [Category("Booking")]
    public class BookingSmokeTest : ICheckSmoke
    {
        public string SmokeTestName => "Always working DB smoke test";
        public string Description => "Succeeding Smoke test for testing purpose";
        public Task<SmokeTestResult> Scenario()
        {
            return Task.FromResult(new SmokeTestResult(true));
        }
    }
}