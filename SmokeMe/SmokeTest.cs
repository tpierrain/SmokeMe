using System.Threading.Tasks;

namespace SmokeMe
{
    /// <summary>
    /// Smoke test/scenario/code to be executed in order to check that a minimum
    /// viable capability of your system is working.
    /// 
    /// Note: all the services and dependencies you need for it will be automatically
    /// injected by the SmokeMe framework via the ASP.NET IServiceProvider of your API
    /// (classical constructor-based injection). Can't be that easy, right? ;-)
    /// </summary>
    public abstract class SmokeTest
    {
        /// <summary>
        /// Name of the smoke test scenario.
        /// </summary>
        public abstract string SmokeTestName { get; }

        /// <summary>
        /// Description of the smoke test scenario.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// The code of this smoke test scenario.
        /// </summary>
        /// <returns>The <see cref="SmokeTestResult"/> of this Smoke test.</returns>
        public abstract Task<SmokeTestResult> Scenario();

        /// <summary>
        /// Returns a value indicating whether or not this smoke test must be discarded (may be interesting to coupled with feature toggle mechanism).
        /// </summary>
        public virtual async Task<bool> HasToBeDiscarded()
        {
            return await Task.FromResult(false);
        }

        /// <summary>
        /// Optional clean-up logic executed after <see cref="Scenario"/>, regardless of its outcome.
        /// Override this to remove any production data created during the smoke test.
        ///
        /// Note: if CleanUp throws, the test outcome is NOT affected — the error is reported
        /// separately via the <c>CleanupError</c> field in the response.
        /// CleanUp is NOT called when the test is discarded (via <see cref="HasToBeDiscarded"/>).
        /// </summary>
        public virtual Task CleanUp()
        {
            return Task.CompletedTask;
        }
    }
}