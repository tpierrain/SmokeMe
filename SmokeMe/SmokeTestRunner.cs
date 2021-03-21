using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
            var tasks = new List<Task<SmokeTestWithItsResultWithMetaData>>();
            foreach (var smokeTest in smokeTests)
            {
                var task = Task.Run(() => StopWatchSafeSmokeTestExecution(smokeTest));
                tasks.Add(task);
            }

            var allSmokeTasks = Task.WhenAll(tasks);
            var timeoutTask = Task.Delay(globalTimeout);

            if (timeoutTask == await Task.WhenAny(timeoutTask, allSmokeTasks).ConfigureAwait(false))
            {
                if (IsNotAFalsePositive(allSmokeTasks)) // in case they all complete in a short
                {
                    var timeoutAndCompletedResultsWithMetadata = await ConcatInferedTimeoutTestsResultsWithCompletedTestsResults(smokeTests, tasks);

                    return new TimeoutSmokeTestsSessionReport(globalTimeout, timeoutAndCompletedResultsWithMetadata, $"One or more smoke tests have timeout (global timeout is: {globalTimeout.GetHumanReadableVersion()})");
                }
            }

            var smokeTestWithItsResultWithMetaDatas = await allSmokeTasks;

            return new SmokeTestsSessionReport(smokeTestWithItsResultWithMetaDatas.Select(x => x.SmokeTestResultWithMetaData).ToArray());
        }

        private static async Task<SmokeTestResultWithMetaData[]> ConcatInferedTimeoutTestsResultsWithCompletedTestsResults(IEnumerable<ICheckSmoke> smokeTests, IEnumerable<Task<SmokeTestWithItsResultWithMetaData>> tasks)
        {
            var completedTasks = tasks.Where(s => s.IsCompleted).ToArray();
            var completedResults = await GetSmokeTestsResultsThatHaveCompleted(completedTasks);
            var completedTestsResultWithMetaDatas = completedResults.Select(x => x.SmokeTestResultWithMetaData).ToArray();

            var timeoutSmokeTests = smokeTests.Except(completedResults.Select(x => x.SmokeTest));
            var timeoutResultWithSomeMetaData = timeoutSmokeTests.Select(x =>
                new SmokeTestResultWithMetaData(new SmokeTestResult(false), null, x.SmokeTestName, x.Description));

            var timeoutAndCompletedResultsWithMetadata =
                timeoutResultWithSomeMetaData.Concat(completedTestsResultWithMetaDatas).ToArray();
            return timeoutAndCompletedResultsWithMetadata;
        }

        private static async Task<List<SmokeTestWithItsResultWithMetaData>> GetSmokeTestsResultsThatHaveCompleted(IEnumerable<Task<SmokeTestWithItsResultWithMetaData>> completedTasks)
        {
            var completedResults = new List<SmokeTestWithItsResultWithMetaData>();
            try
            {
                completedResults.AddRange(await Task.WhenAll(completedTasks));
            }
            catch
            {
            }

            return completedResults;
        }

        private static bool IsNotAFalsePositive(Task allSmokeTasks)
        {
            return !allSmokeTasks.IsCompletedSuccessfully;
        }

        private static async Task<SmokeTestWithItsResultWithMetaData> StopWatchSafeSmokeTestExecution(ICheckSmoke smokeTest)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var smokeTestResult = await smokeTest.Scenario();

                stopwatch.Stop();
                var smokeTestExecution = WrapSmokeTestResultWithMetaData(smokeTestResult, stopwatch.Elapsed, smokeTest);

                return new SmokeTestWithItsResultWithMetaData(smokeTest , smokeTestExecution);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                var smokeTestResult = new SmokeTestResult("", ex);
                var smokeTestExecution = WrapSmokeTestResultWithMetaData(smokeTestResult, stopwatch.Elapsed, smokeTest);
                return new SmokeTestWithItsResultWithMetaData(smokeTest, smokeTestExecution);
            }
        }

        private static SmokeTestResultWithMetaData WrapSmokeTestResultWithMetaData(SmokeTestResult smokeTestResult, TimeSpan elapsedTime, ICheckSmoke smokeTest)
        {
            return new SmokeTestResultWithMetaData(smokeTestResult, elapsedTime, smokeTest.SmokeTestName, smokeTest.Description);
        }

        private class SmokeTestWithItsResultWithMetaData
        {
            public ICheckSmoke SmokeTest { get; }
            public SmokeTestResultWithMetaData SmokeTestResultWithMetaData { get; }

            public SmokeTestWithItsResultWithMetaData(ICheckSmoke smokeTest, SmokeTestResultWithMetaData smokeTestResultWithMetaData)
            {
                SmokeTest = smokeTest;
                SmokeTestResultWithMetaData = smokeTestResultWithMetaData;
            }
        }
    }
}