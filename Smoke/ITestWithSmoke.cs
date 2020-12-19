namespace Smoke
{
    /// <summary>
    /// Contains scenario to be executed in order to 'smoke test' something.
    /// </summary>
    public interface ITestWithSmoke
    {

        SmokeTestResult Execute();
    }
}