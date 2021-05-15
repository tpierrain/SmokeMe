using System;
using System.Threading;
using System.Threading.Tasks;
using Sample.ExternalSmokeTests.Utilities;
using SmokeMe.Tests.Helpers;

namespace SmokeMe.Tests.SmokeTests
{
    public class FeatureToggledAlwaysPositiveSmokeTest : SmokeTest
    {
        private readonly IToggleFeatures _featureToggles;
        private readonly TimeSpan _delay;

        public FeatureToggledAlwaysPositiveSmokeTest(IToggleFeatures featureToggles, TimeSpan delay)
        {
            _featureToggles = featureToggles;
            _delay = delay;
        }

        public override Task<bool> HasToBeDiscarded()
        {
            return Task.FromResult(!_featureToggles.IsEnabled("featureToggledSmokeTest"));
        }

        public override string SmokeTestName => "Feature toggled test";
        public override string Description { get; }

        public override Task<SmokeTestResult> Scenario()
        {
            Thread.Sleep(Convert.ToInt32(_delay.TotalMilliseconds));
            return Task.FromResult(new SmokeTestResult(true));
        }
    }


    [Ignore]

    public class FeatureToggledSmokeTest : SmokeTest
    {
        private readonly IToggleFeatures _featureToggles;

        public FeatureToggledSmokeTest(IToggleFeatures featureToggles)
        {
            _featureToggles = featureToggles;
        }

        public override Task<bool> HasToBeDiscarded()
        {
            return Task.FromResult(!_featureToggles.IsEnabled("featureToggledSmokeTest"));
        }

        public override string SmokeTestName => "Dummy but feature toggled smoke test";
        public override string Description => "A smoke test in order to show how to Discard or not based on your own feature toggle system";

        public override Task<SmokeTestResult> Scenario()
        {
            return Task.FromResult(new SmokeTestResult(true));
        }
    }
}