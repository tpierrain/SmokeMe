using System.Collections.Generic;

namespace Smoke
{
    /// <summary>
    /// Responsible to find smoke tests to be run within an executable.
    /// </summary>
    public class SmokeTestAutoFinder : IFindSmokeTests
    {

        public IEnumerable<ISmokeTestAScenario> FindAllSmokeTestsToRun()
        {
            var smokeTestInstances = new List<ISmokeTestAScenario>();

            // Search all types implementing the ISmokeTestAScenario interface

            // Instantiate them using the IoC

            return smokeTestInstances;
        }
    }
}