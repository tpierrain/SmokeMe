using System.Collections.Generic;

namespace Smoke
{
    /// <summary>
    /// Responsible to find smoke tests to be run within an executable.
    /// </summary>
    public interface IFindSmokeTests
    {
        /// <summary>
        /// Instantiates all the <see cref="ISmokeTestAScenario"/> instances that have been found in the running code.
        /// </summary>
        /// <returns>A collection of <see cref="ISmokeTestAScenario"/> instances.</returns>
        IEnumerable<ISmokeTestAScenario> FindAllSmokeTestsToRun();
    }
}