using System.Threading.Tasks;
using SmokeMe;

namespace Sample.ExternalSmokeTests
{
    [Category("Awkward")]
    [Ignore]
    public class AlwaysWorkingButIgnoredDbSmokeTest : SmokeTest
    {
        public override string SmokeTestName => "Always working but ignored DB smoke test";
        public override string Description => "Succeeding Smoke test for testing purpose";

        public override Task<SmokeTestResult> Scenario()
        {
            return Task.FromResult(new SmokeTestResult(true));
        }
    }
}