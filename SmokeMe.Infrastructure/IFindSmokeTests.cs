namespace SmokeMe
{
    /// <summary>
    /// Responsible to find smoke tests to be run within an executable.
    /// </summary>
    public interface IFindSmokeTests
    {
        /// <summary>
        /// Instantiates all the <see cref="SmokeTest"/> instances that have been found in the running code.
        /// </summary>
        /// <returns>A collection of <see cref="SmokeTest"/> instances.</returns>
        SmokeTestInstanceWithMetaData[] FindAllSmokeTestsToRun(params string[] requestedCategories);
    }
}