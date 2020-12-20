using System;
using System.Threading.Tasks;
using SmokeMe;

namespace Sample.dotnet5.Api
{
    public class NullSmokeTest : ICheckSmoke
    {
        public string SmokeTestName => "Null smoke test";
        public string Description => "This is a dummy smoke test doing nothing more than waiting 0.025 ms.";
        public async Task<SmokeTestResult> Scenario()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(0.025));

            return new SmokeTestResult(true);
        }
    }
}