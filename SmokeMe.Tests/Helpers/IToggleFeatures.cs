namespace SmokeMe.Tests.Helpers
{
    public interface IToggleFeatures
    {
        bool IsEnabled(string featureName);
    }
}