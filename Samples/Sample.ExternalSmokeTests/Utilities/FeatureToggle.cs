namespace Sample.ExternalSmokeTests.Utilities
{
    public class FeatureToggle
    {
        public string FeatureName { get; }
        public bool FeatureValue { get; }

        public FeatureToggle(string featureName, in bool featureValue)
        {
            FeatureName = featureName;
            FeatureValue = featureValue;
        }
    }
}