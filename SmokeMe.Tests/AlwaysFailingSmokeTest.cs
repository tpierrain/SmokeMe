using System.Threading.Tasks;

namespace SmokeMe.Tests
{
    [Category("FailingSaMere")]
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