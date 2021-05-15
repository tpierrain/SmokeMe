using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SmokeMe.Helpers;

namespace SmokeMe
{
    /// <summary>
    /// Runner for <see cref="SmokeTest"/> instances.
    /// </summary>
    public class SmokeTestRunner
    {
        /// <summary>
        /// Executes <see cref="SmokeTest"/> instances that has been found for this API.
        /// </summary>
        /// <param name="smokeTestsWithMetaData">The <see cref="SmokeTestInstanceWithMetaData"/> instances to be executed in parallel.</param>
        /// <param name="globalTimeout">The maximum amount of time allowed for all <see cref="SmokeTest"/> instances to be executed.</param>
        /// <returns>The <see cref="SmokeTestsSessionReport"/>.</returns>
        public static async Task<SmokeTestsSessionReport> ExecuteAllSmokeTestsInParallel(SmokeTestInstanceWithMetaData[] smokeTestsWithMetaData, TimeSpan globalTimeout)
        {
            var unignoredSmokeTestsWithMetadata = smokeTestsWithMetaData.Where(s => !s.SmokeTest.GetType().HasIgnoredCustomAttribute());

            var tasks = new List<Task<SmokeTestWithItsResultWithMetaData>>();
            foreach (var smokeTestWithMetaData in unignoredSmokeTestsWithMetadata.ToArray())
            {
                if (!smokeTestWithMetaData.SmokeTest.GetType().HasIgnoredCustomAttribute())
                {
                    var task = Task.Run(() => StopWatchSafeSmokeTestExecution(smokeTestWithMetaData));
                    tasks.Add(task);
                }
            }

            var allSmokeTasks = Task.WhenAll(tasks);
            var timeoutTask = Task.Delay(globalTimeout);

            var ignoredSmokeTestsWithMetadata = smokeTestsWithMetaData
                .Where(s => s.SmokeTest.GetType().HasIgnoredCustomAttribute())
                .Select(x => new SmokeTestResultWithMetaData(new SmokeTestResult(true), TimeSpan.Zero, new SmokeTestInstanceWithMetaData(x.SmokeTest, x.SmokeTest.GetCategories()), x.SmokeTest.GetType().FullName, discarded: false, status: Status.Ignored))
                .ToArray();

            if (timeoutTask == await Task.WhenAny(timeoutTask, allSmokeTasks).ConfigureAwait(false))
            {
                if (IsNotAFalsePositive(allSmokeTasks)) // in case they all complete in a short
                {
                    var timeoutAndCompletedResultsWithMetadata = await ConcatDeducedTimeoutTestsResultsWithCompletedTestsResults(unignoredSmokeTestsWithMetadata, tasks);
                    var allResults = timeoutAndCompletedResultsWithMetadata.Concat(ignoredSmokeTestsWithMetadata).ToArray();

                    return new TimeoutSmokeTestsSessionReport(globalTimeout, allResults, $"One or more smoke tests have timeout (global timeout is: {globalTimeout.GetHumanReadableVersion()})");
                }
            }

            var smokeTestWithItsResultWithMetaDatas = await allSmokeTasks;

            //var ignoredSmokeTestsWithResultsWithMetadata = smokeTestsWithMetaData
            //    .Where(s => s.SmokeTest.GetType().HasIgnoredCustomAttribute())
            //    .Select(x => new SmokeTestWithItsResultWithMetaData(x.SmokeTest, new SmokeTestResultWithMetaData(new SmokeTestResult(true), TimeSpan.Zero, new SmokeTestInstanceWithMetaData(x.SmokeTest, x.SmokeTest.GetCategories()), x.SmokeTest.GetType().FullName, discarded:false, Status.Ignored), x.SmokeTestIdentifier.Value))
            //    .ToArray();


            var smokeTestResultWithMetaDatas = smokeTestWithItsResultWithMetaDatas
                                                    .Select(x => x.SmokeTestResultWithMetaData)
                                                    .ToArray();

            var allResults2 = smokeTestResultWithMetaDatas.Concat(ignoredSmokeTestsWithMetadata).ToArray();

            return new SmokeTestsSessionReport(allResults2);
        }

        private static async Task<SmokeTestResultWithMetaData[]> ConcatDeducedTimeoutTestsResultsWithCompletedTestsResults(IEnumerable<SmokeTestInstanceWithMetaData> smokeTestsWithMetaData, IEnumerable<Task<SmokeTestWithItsResultWithMetaData>> tasks)
        {
            // TODO: refactor this! (keep only one type: SmokeTestWithItsResultWithMetaData or SmokeTestResultWithMetaData)

            IEnumerable<Task<SmokeTestWithItsResultWithMetaData>> completedTasks = tasks.Where(s => s.IsCompleted).ToArray();
            
            List<SmokeTestWithItsResultWithMetaData> completedResults = await GetSmokeTestsResultsThatHaveCompleted(completedTasks);
            
            SmokeTestResultWithMetaData[] completedTestsResultWithMetaData = completedResults.Select(x => x.SmokeTestResultWithMetaData).ToArray();

            var completedIdentifiers = completedResults.Select(x => x.SmokeTestIdentifier).ToArray();

            var timeoutSmokeTests = smokeTestsWithMetaData.Where(x => !completedIdentifiers.Contains(x.SmokeTestIdentifier.Value));

            var timeoutResultWithSomeMetaData = timeoutSmokeTests.Select(x => new SmokeTestResultWithMetaData(new SmokeTestResult(false), null, x, smokeTestType:x.SmokeTest.GetType().FullName, status: Status.Timeout));

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
                if (await smokeTest.HasToBeDiscarded() == true)
                {
                    stopwatch.Stop();

                    var smokeTestNonExecution = new SmokeTestResultWithMetaData(new SmokeTestResult(true), stopwatch.Elapsed, smokeTestWithMetaData, smokeTestType: smokeTest.GetType().FullName, discarded: true);
                    return new SmokeTestWithItsResultWithMetaData(smokeTest, smokeTestNonExecution, smokeTestWithMetaData.SmokeTestIdentifier.Value);
                }

                var smokeTestResult = await smokeTest.Scenario();

                stopwatch.Stop();
                var smokeTestExecution = WrapSmokeTestResultWithMetaData(smokeTestResult, stopwatch.Elapsed, smokeTestWithMetaData, smokeTestTypeName:smokeTest.GetType().FullName);

                return new SmokeTestWithItsResultWithMetaData(smokeTest , smokeTestExecution, smokeTestWithMetaData.SmokeTestIdentifier.Value);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                var smokeTestResult = new SmokeTestResult("", ex);
                var smokeTestExecution = WrapSmokeTestResultWithMetaData(smokeTestResult, stopwatch.Elapsed, smokeTestWithMetaData, smokeTestTypeName: smokeTest.GetType().FullName);
                return new SmokeTestWithItsResultWithMetaData(smokeTest, smokeTestExecution, smokeTestWithMetaData.SmokeTestIdentifier.Value);
            }
        }

        private static SmokeTestResultWithMetaData WrapSmokeTestResultWithMetaData(SmokeTestResult smokeTestResult, TimeSpan elapsedTime, SmokeTestInstanceWithMetaData smokeTestInstanceWithMetaData, string smokeTestTypeName)
        {
            return new SmokeTestResultWithMetaData(smokeTestResult, elapsedTime, smokeTestInstanceWithMetaData, smokeTestType: smokeTestTypeName);
        }

        private class SmokeTestWithItsResultWithMetaData
        {
            public SmokeTest SmokeTest { get; }

            public SmokeTestResultWithMetaData SmokeTestResultWithMetaData { get; }
            
            public int SmokeTestIdentifier { get; }

            public SmokeTestWithItsResultWithMetaData(SmokeTest smokeTest, SmokeTestResultWithMetaData smokeTestResultWithMetaData, int smokeTestIdentifier)
            {
                SmokeTest = smokeTest;
                SmokeTestResultWithMetaData = smokeTestResultWithMetaData;
                SmokeTestIdentifier = smokeTestIdentifier;
            }
        }
    }
}