using System.Threading.Tasks;

namespace SmokeMe.Tests.SmokeTests
{
    internal class SmokeTestWithSuccessfulCleanUp : SmokeTest
    {
        public override string SmokeTestName => "Smoke test with successful cleanup";
        public override string Description => "For unit testing purpose. CleanUp succeeds.";

        public bool CleanUpWasCalled { get; private set; }

        public override Task<SmokeTestResult> Scenario()
        {
            return Task.FromResult(new SmokeTestResult(true));
        }

        public override Task CleanUp()
        {
            CleanUpWasCalled = true;
            return Task.CompletedTask;
        }
    }
}
