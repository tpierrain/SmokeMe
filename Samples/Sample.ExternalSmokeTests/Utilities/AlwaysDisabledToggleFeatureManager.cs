namespace Sample.ExternalSmokeTests.Utilities
{
    public class AlwaysDisabledToggleFeatureManager : IToggleFeatures
    {
        public bool IsEnabled(string featureName)
        {
            return false;
        }
    }
}