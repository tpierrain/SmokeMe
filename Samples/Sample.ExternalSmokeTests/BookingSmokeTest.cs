using System.Threading.Tasks;
using SmokeMe;

namespace Sample.ExternalSmokeTests
{
    [Category("DB")]
    [Category("Booking")]
    public class BookingSmokeTest : SmokeTest
    {
        public override string SmokeTestName => "Always working DB smoke test";
        public override string Description => "Succeeding Smoke test for testing purpose";

        public override Task<SmokeTestResult> Scenario()
        {
            return Task.FromResult(new SmokeTestResult(true));
        }
    }
}