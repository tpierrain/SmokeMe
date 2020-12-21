using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using SmokeMe.Helpers;

namespace SmokeMe
{
    /// <summary>
    /// Runner for <see cref="ICheckSmoke"/> instances.
    /// </summary>
    public class SmokeTestRunner
    {
        /// <summary>
        /// Executes <see cref="ICheckSmoke"/> instances that has been found for this API.
        /// </summary>
        /// <param name="smokeTests">The <see cref="ICheckSmoke"/> instances to be executed in parallel.</param>
        /// <param name="globalTimeout">The maximum amount of time allowed for all <see cref="ICheckSmoke"/> instances to be executed.</param>
        /// <returns>The <see cref="SmokeTestsSessionReport"/>.</returns>
        public static async Task<SmokeTestsSessionReport> ExecuteAllSmokeTestsInParallel(IEnumerable<ICheckSmoke> smokeTests, TimeSpan globalTimeout)
        {
            var tasks = new List<Task<SmokeTestResultWithMetaData>>();
            foreach (var smokeTest in smokeTests)
            {
                var task = Task.Run(() => StopWatchSafeSmokeTestExecution(smokeTest));
                tasks.Add(task);
            }

            var allSmokeTasks = Task.WhenAll(tasks);
            var timeoutTask = Task.Delay(globalTimeout);
            //var timeoutTask = Task.Run(() => Thread.Sleep(globalTimeout));

            if (timeoutTask == await Task.WhenAny(timeoutTask, allSmokeTasks).ConfigureAwait(false))
            {
                if (IsNotAFalsePositive(allSmokeTasks)) // in case they all complete in a short
                {
                    return new TimeoutSmokeTestsSessionReport(globalTimeout, $"One or more smoke tests have timeout (global timeout is: {globalTimeout.GetHumanReadableVersion()})");
                }
            }

            return new SmokeTestsSessionReport(await allSmokeTasks);
        }

        private static bool IsNotAFalsePositive(Task<SmokeTestResultWithMetaData[]> allSmokeTasks)
        {
            return !allSmokeTasks.IsCompletedSuccessfully;
        }

        private static async Task<SmokeTestResultWithMetaData> StopWatchSafeSmokeTestExecution(ICheckSmoke smokeTest)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var smokeTestResult = await smokeTest.Scenario();

                stopwatch.Stop();
                var smokeTestExecution = WrapSmokeTestResultWithMetaData(smokeTestResult, stopwatch.Elapsed, smokeTest);

                return smokeTestExecution;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                var smokeTestResult = new SmokeTestResult("", ex);
                var smokeTestExecution = WrapSmokeTestResultWithMetaData(smokeTestResult, stopwatch.Elapsed, smokeTest);
                return smokeTestExecution;
            }
        }

        private static SmokeTestResultWithMetaData WrapSmokeTestResultWithMetaData(SmokeTestResult smokeTestResult, TimeSpan elapsedTime, ICheckSmoke smokeTest)
        {
            return new SmokeTestResultWithMetaData(smokeTestResult, elapsedTime, smokeTest.SmokeTestName, smokeTest.Description);
        }
    }
}