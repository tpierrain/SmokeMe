using System.Threading.Tasks;

namespace SmokeMe.Tests.SmokeTests
{
    [SmokeMe.Category("FailingSaMere")]
    public class AlwaysFailingSmokeTest : SmokeTest
    {
        public override string SmokeTestName => "Failing on purpose";
        public override string Description => "Failing Smoke test for testing purpose";

        public override Task<SmokeTestResult> Scenario()
        {
            throw new System.NotImplementedException();
        }
    }
}