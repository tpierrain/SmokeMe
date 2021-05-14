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
        /// Gets a value indicating whether or not this smoke test must be discarded (may be interesting to coupled with feature toggle mechanism).
        /// </summary>
        public virtual bool MustBeDiscarded => false;
    }
}