using System.ComponentModel;
using System.Threading.Tasks;
using SmokeMe;

namespace Sample.ExternalSmokeTests
{
    [SmokeTestCategory("FailingSaMere")]
    public class AlwaysFailingSmokeTest : ICheckSmoke
    {
        public string SmokeTestName => "Failing on purpose";
        public string Description => "Failing Smoke test for testing purpose";
        public Task<SmokeTestResult> Scenario()
        {
            throw new System.NotImplementedException();
        }
    }
}