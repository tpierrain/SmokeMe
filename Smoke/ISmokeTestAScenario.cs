namespace Smoke
{
    /// <summary>
    /// Contains scenario to be executed in order to 'smoke test' something.
    /// (a Smoke test actually).
    /// Note: all the services and dependencies you need for it will be automatically
    /// injected by the lib via the ASP.NET IServiceProvider of your API
    /// (classical constructor-based injection).
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