using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SmokeMe
{
    /// <summary>
    /// Runner for <see cref="ISmokeTestAScenario"/> instances.
    /// </summary>
    public class SmokeTestRunner
    {
        /// <summary>
        /// Executes <see cref="ISmokeTestAScenario"/> instances that has been found for this API.
        /// </summary>
        /// <param name="smokeTests">The <see cref="ISmokeTestAScenario"/> instances to be executed in parallel.</param>
        /// <param name="globalTimeout">The maximum amount of time allowed for all <see cref="ISmokeTestAScenario"/> instances to be executed.</param>
        /// <returns>The <see cref="SmokeTestSessionResult"/>.</returns>
        public static async Task<SmokeTestSessionResult> ExecuteAllSmokeTests(IEnumerable<ISmokeTestAScenario> smokeTests, TimeSpan globalTimeout)
        {
            var tasks = new List<Task<StopWatchedSmokeTestExecution>>();
            foreach (var smokeTest in smokeTests)
            {
                var task = Task.Run(() => StopWatchSafeSmokeTestExecution(smokeTest));
                tasks.Add(task);
            }

            var allSmokeTasks = Task.WhenAll(tasks);
            var timeoutTask = Task.Run(() => Thread.Sleep(globalTimeout));

            if (timeoutTask == await Task.WhenAny(timeoutTask, allSmokeTasks).ConfigureAwait(false))
            {
                if (IsNotAFalsePositive(allSmokeTasks)) // in case they all complete in a short
                {
                    return new TimeoutSmokeTestSessionResult(globalTimeout);
                }
            }

            return new SmokeTestSessionResult(await allSmokeTasks);
        }

        private static bool IsNotAFalsePositive(Task<StopWatchedSmokeTestExecution[]> allSmokeTasks)
        {
            return !allSmokeTasks.IsCompletedSuccessfully;
        }

        private static StopWatchedSmokeTestExecution StopWatchSafeSmokeTestExecution(ISmokeTestAScenario smokeTest)
        {
            var stopwatch = new Stopwatch();
            try
            {
                var smokeTestResult = smokeTest.ExecuteScenario();
                stopwatch.Stop();
                var smokeTestExecution = new StopWatchedSmokeTestExecution(smokeTestResult, stopwatch.Elapsed);

                return smokeTestExecution;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                var smokeTestResult = new SmokeTestResult("", ex);
                var smokeTestExecution = new StopWatchedSmokeTestExecution(smokeTestResult, stopwatch.Elapsed);
                return smokeTestExecution;
            }
        }
    }

    /// <summary>
    /// Represents a failed (due to timeout) smoke test session.
    /// </summary>
    public class TimeoutSmokeTestSessionResult : SmokeTestSessionResult
    {
        private readonly TimeSpan _globalTimeout;

        /// <summary>
        /// Instantiates a <see cref="TimeoutSmokeTestSessionResult"/>.
        /// </summary>
        /// <param name="globalTimeout">The global timeout expiration that led to his failure.</param>
        public TimeoutSmokeTestSessionResult(TimeSpan globalTimeout) : base(new StopWatchedSmokeTestExecution[0], false)
        {
            _globalTimeout = globalTimeout;
        }

    }
}