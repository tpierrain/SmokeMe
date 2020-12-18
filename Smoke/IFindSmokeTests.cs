﻿using System.Collections.Generic;

namespace Smoke
{
    /// <summary>
    /// Responsible to find smoke tests to be run within an executable.
    /// </summary>
    public interface IFindSmokeTests
    {
        IEnumerable<ISmokeTestAScenario> FindAllSmokeTestsToRun();
    }

    public class SmokeTestFinder : IFindSmokeTests
    {
        public IEnumerable<ISmokeTestAScenario> FindAllSmokeTestsToRun()
        {
            return new ISmokeTestAScenario[0];
        }
    }
}