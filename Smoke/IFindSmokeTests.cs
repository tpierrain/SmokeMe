using System.Collections.Generic;

namespace Smoke
{
    /// <summary>
    /// Responsible to find smoke tests to be run within an executable.
    /// </summary>
    public interface IFindSmokeTests
    {
        IEnumerable<ITestWithSmoke> FindAllSmokeTestsToRun();
    }
}