using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// <param name="smokeTestsWithMetaData">The <see cref="SmokeTestInstanceWithMetaData"/> instances to be executed in parallel.</param>
        /// <param name="globalTimeout">The maximum amount of time allowed for all <see cref="ICheckSmoke"/> instances to be executed.</param>
        /// <returns>The <see cref="SmokeTestsSessionReport"/>.</returns>
        public static async Task<SmokeTestsSessionReport> ExecuteAllSmokeTestsInParallel(IEnumerable<SmokeTestInstanceWithMetaData> smokeTestsWithMetaData, TimeSpan globalTimeout)
        {
            var tasks = new List<Task<SmokeTestWithItsResultWithMetaData>>();
            foreach (var smokeTestWithMetaData in smokeTestsWithMetaData.ToArray())
            {
                var task = Task.Run(() => StopWatchSafeSmokeTestExecution(smokeTestWithMetaData));
                tasks.Add(task);
            }

            var allSmokeTasks = Task.WhenAll(tasks);
            var timeoutTask = Task.Delay(globalTimeout);

            if (timeoutTask == await Task.WhenAny(timeoutTask, allSmokeTasks).ConfigureAwait(false))
            {
                if (IsNotAFalsePositive(allSmokeTasks)) // in case they all complete in a short
                {
                    var timeoutAndCompletedResultsWithMetadata = await ConcatDeducedTimeoutTestsResultsWithCompletedTestsResults(smokeTestsWithMetaData, tasks);

                    return new TimeoutSmokeTestsSessionReport(globalTimeout, timeoutAndCompletedResultsWithMetadata, $"One or more smoke tests have timeout (global timeout is: {globalTimeout.GetHumanReadableVersion()})");
                }
            }

            var smokeTestWithItsResultWithMetaDatas = await allSmokeTasks;

            return new SmokeTestsSessionReport(smokeTestWithItsResultWithMetaDatas.Select(x => x.SmokeTestResultWithMetaData).ToArray());
        }

        private static async Task<SmokeTestResultWithMetaData[]> ConcatDeducedTimeoutTestsResultsWithCompletedTestsResults(IEnumerable<SmokeTestInstanceWithMetaData> smokeTestsWithMetaData, IEnumerable<Task<SmokeTestWithItsResultWithMetaData>> tasks)
        {
            // TODO: refactor this! (keep only one type: SmokeTestWithItsResultWithMetaData or SmokeTestResultWithMetaData)

            IEnumerable<Task<SmokeTestWithItsResultWithMetaData>> completedTasks = tasks.Where(s => s.IsCompleted).ToArray();
            
            List<SmokeTestWithItsResultWithMetaData> completedResults = await GetSmokeTestsResultsThatHaveCompleted(completedTasks);
            
            SmokeTestResultWithMetaData[] completedTestsResultWithMetaData = completedResults.Select(x => x.SmokeTestResultWithMetaData).ToArray();

            var completedIdentifiers = completedResults.Select(x => x.SmokeTestIdentifier).ToArray();

            var timeoutSmokeTests = smokeTestsWithMetaData.Where(x => !completedIdentifiers.Contains(x.SmokeTestIdentifier.Value));

            var timeoutResultWithSomeMetaData = timeoutSmokeTests.Select(x => new SmokeTestResultWithMetaData(new SmokeTestResult(false), null, x));

            var timeoutAndCompletedResultsWithMetadata = timeoutResultWithSomeMetaData.Concat(completedTestsResultWithMetaData).ToArray();

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

        private static async Task<SmokeTestWithItsResultWithMetaData> StopWatchSafeSmokeTestExecution(SmokeTestInstanceWithMetaData smokeTestWithMetaData)
        {
            var smokeTest = smokeTestWithMetaData.SmokeTest;

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var smokeTestResult = await smokeTest.Scenario();

                stopwatch.Stop();
                var smokeTestExecution = WrapSmokeTestResultWithMetaData(smokeTestResult, stopwatch.Elapsed, smokeTestWithMetaData);

                return new SmokeTestWithItsResultWithMetaData(smokeTest , smokeTestExecution, smokeTestWithMetaData.SmokeTestIdentifier.Value);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                var smokeTestResult = new SmokeTestResult("", ex);
                var smokeTestExecution = WrapSmokeTestResultWithMetaData(smokeTestResult, stopwatch.Elapsed, smokeTestWithMetaData);
                return new SmokeTestWithItsResultWithMetaData(smokeTest, smokeTestExecution, smokeTestWithMetaData.SmokeTestIdentifier.Value);
            }
        }

        private static SmokeTestResultWithMetaData WrapSmokeTestResultWithMetaData(SmokeTestResult smokeTestResult, TimeSpan elapsedTime, SmokeTestInstanceWithMetaData smokeTestInstanceWithMetaData)
        {
            return new SmokeTestResultWithMetaData(smokeTestResult, elapsedTime, smokeTestInstanceWithMetaData);
        }

        private class SmokeTestWithItsResultWithMetaData
        {
            public ICheckSmoke SmokeTest { get; }

            public SmokeTestResultWithMetaData SmokeTestResultWithMetaData { get; }
            
            public int SmokeTestIdentifier { get; }

            public SmokeTestWithItsResultWithMetaData(ICheckSmoke smokeTest, SmokeTestResultWithMetaData smokeTestResultWithMetaData, int smokeTestIdentifier)
            {
                SmokeTest = smokeTest;
                SmokeTestResultWithMetaData = smokeTestResultWithMetaData;
                SmokeTestIdentifier = smokeTestIdentifier;
            }
        }
    }
}