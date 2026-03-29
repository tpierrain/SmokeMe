using System;
using System.Threading.Tasks;

namespace SmokeMe.Tests.SmokeTests
{
    internal class SmokeTestWithFailingCleanUp : SmokeTest
    {
        private readonly bool _scenarioSucceeds;

        public override string SmokeTestName => "Smoke test with failing cleanup";
        public override string Description => "For unit testing purpose. CleanUp always throws.";

        public bool CleanUpWasCalled { get; private set; }

        public SmokeTestWithFailingCleanUp(bool scenarioSucceeds = true)
        {
            _scenarioSucceeds = scenarioSucceeds;
        }

        public override Task<SmokeTestResult> Scenario()
        {
            if (!_scenarioSucceeds)
            {
                throw new InvalidOperationException("Scenario failed on purpose");
            }

            return Task.FromResult(new SmokeTestResult(true));
        }

        public override Task CleanUp()
        {
            CleanUpWasCalled = true;
            throw new InvalidOperationException("CleanUp failed on purpose");
        }
    }
}
