﻿using System.Threading.Tasks;

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
    public interface ICheckSmoke
    {
        /// <summary>
        /// The code of this smoke test scenario.
        /// </summary>
        /// <returns>The <see cref="SmokeTestResult"/> of this Smoke test.</returns>
        Task<SmokeTestResult> Scenario();

        /// <summary>
        /// Name of the smoke test scenario.
        /// </summary>
        string SmokeTestName { get; }

        /// <summary>
        /// Description of the smoke test scenario.
        /// </summary>
        string Description { get; }
    }
}