namespace Sample.ExternalSmokeTests.Utilities
{
    public interface IToggleFeatures
    {
        bool IsEnabled(string featureName);
    }
}