namespace Smoke
{
    /// <summary>
    /// Contains code to be run in order to smoke test something.
    /// </summary>
    public interface ISmokeTestAScenario
    {
        SmokeTestResult RunSmokeTest();
    }
}