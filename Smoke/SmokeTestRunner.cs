using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Smoke
{
    /// <summary>
    /// Runner for <see cref="ISmokeTestAScenario"/> instances.
    /// </summary>
    public class SmokeTestRunner
    {
        public static async Task<SmokeTestSessionResult> ExecuteAllSmokeTests(IEnumerable<ISmokeTestAScenario> smokeTests, TimeSpan globalTimeout)
        {
            var tasks = new List<Task<StopWatchedSmokeTestExecution>>();
            foreach (var smokeTest in smokeTests)
            {
                var task = Task.Run(() =>
                {
                    var stopwatch = new Stopwatch();
                    try
                    {
                        var smokeTestResult = smokeTest.RunSmokeTest();
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
                });

                tasks.Add(task);
            }

            var timeoutTask = Task.Run(() => Thread.Sleep(globalTimeout.Milliseconds));
            var allSmokeTasks = Task.WhenAll(tasks);

            if (await Task.WhenAny(timeoutTask, allSmokeTasks) == timeoutTask)
            {
                return new TimeoutSmokeTestSessionResult(globalTimeout);
            }

            return new SmokeTestSessionResult(await allSmokeTasks);
        }
    }

    public class TimeoutSmokeTestSessionResult : SmokeTestSessionResult
    {
        private readonly TimeSpan _globalTimeout;

        public TimeoutSmokeTestSessionResult(TimeSpan globalTimeout) : base(new StopWatchedSmokeTestExecution[0], false)
        {
            _globalTimeout = globalTimeout;
        }

    }
}