using System;
using System.Collections.Generic;

namespace SmokeMe
{
    /// <summary>
    /// Responsible to find and instantiate smoke tests to be run within an executable.
    /// </summary>
    public class SmokeTestAutoFinder : IFindSmokeTests
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Instantiates a <see cref="SmokeTestAutoFinder"/>
        /// </summary>
        /// <param name="serviceProvider">The (IoC) <see cref="IServiceProvider"/> instance needed to instantiate <see cref="ISmokeTestAScenario"/> instances.</param>
        public SmokeTestAutoFinder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Finds all smoke tests scenarii that have to be executed for this API.
        /// </summary>
        /// <returns>The collection of all <see cref="ISmokeTestAScenario"/> instance declared in this API to be executed.</returns>
        public IEnumerable<ISmokeTestAScenario> FindAllSmokeTestsToRun()
        {
            var smokeTestInstances = new List<ISmokeTestAScenario>();



            //var myself = _serviceProvider.GetRequiredService<T>();
            

            // Search all types implementing the ISmokeTestAScenario interface

            // Instantiate them using the IoC

            return smokeTestInstances;
        }
    }
}