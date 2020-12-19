using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Smoke
{
    /// <summary>
    /// Responsible to find and instantiate smoke tests to be run within an executable.
    /// </summary>
    public class SmokeTestAutoFinder : IFindSmokeTests
    {
        private readonly IServiceProvider _serviceProvider;

        public SmokeTestAutoFinder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<ITestWithSmoke> FindAllSmokeTestsToRun()
        {
            var smokeTestInstances = new List<ITestWithSmoke>();

            //var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            //var serviceProvider = serviceCollection.BuildServiceProvider();
           

            //_serviceProvider.GetRequiredService<IProviderNumbers>();

            var myself = _serviceProvider.GetRequiredService<IFindSmokeTests>();
            if (myself == this)
            {

            }
            else
            {
                
            }

            // Search all types implementing the ITestWithSmoke interface

            // Instantiate them using the IoC

            return smokeTestInstances;
        }
    }
}