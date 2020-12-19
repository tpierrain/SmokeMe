namespace Smoke
{
    /// <summary>
    /// Contains code to be ran in order to 'smoke test' something.
    /// </summary>
    public interface ITestWithSmoke
    {
        SmokeTestResult Run();
    }
}