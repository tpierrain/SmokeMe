namespace Smoke
{
    /// <summary>
    /// Contains scenario to be executed in order to 'smoke test' something.
    /// Smoke test actually.
    /// </summary>
    public interface ISmokeTestAScenario
    {

        /// <summary>
        /// Executes the scenario of this Smoke Test.
        /// </summary>
        /// <returns>The <see cref="SmokeTestResult"/> of this Smoke test.</returns>
        SmokeTestResult ExecuteScenario();
    }
}