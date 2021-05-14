using System;
using System.Threading.Tasks;
using SmokeMe;

namespace Sample.Api.SmokeTests
{
    public class AlwaysDiscardedSmokeTest : SmokeTest
    {
        public override string SmokeTestName => "Discarded Smoke test";
        public override string Description => "Smoke test systematically Discarded";

        public override async Task<bool> HasToBeDiscarded()
        {
            return await Task.FromResult(true);
        }

        public override Task<SmokeTestResult> Scenario()
        {
            throw new NotImplementedException();
        }
    }
}