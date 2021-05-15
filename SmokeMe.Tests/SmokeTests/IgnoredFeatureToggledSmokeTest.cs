using System.Threading.Tasks;
using Sample.ExternalSmokeTests.Utilities;

namespace SmokeMe.Tests.SmokeTests
{
    [Ignore]

    public class IgnoredFeatureToggledSmokeTest : SmokeTest
    {
        private readonly IToggleFeatures _featureToggles;

        public IgnoredFeatureToggledSmokeTest(IToggleFeatures featureToggles)
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