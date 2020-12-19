using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmokeMe.Helpers;

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

            var types = GetTypesImplementing<ISmokeTestAScenario>();
            foreach (var smokeTestType in types)
            {
                var constructors = smokeTestType.GetConstructorsOrderedByNumberOfParametersDesc();
                var smokeTest = InstantiateSmokeTest(constructors, _serviceProvider);
                if (smokeTest != null)
                {
                    smokeTestInstances.Add(smokeTest);
                }
            }
            
            return smokeTestInstances;
        }

        private static ISmokeTestAScenario InstantiateSmokeTest(IEnumerable<ConstructorInfo> constructors, IServiceProvider serviceProvider)
        {
            foreach (var constructor in constructors)
            {
                try
                {
                    var constructorParameters = PrepareParametersForThisConstructor(constructor, serviceProvider);
                    var instance = (ISmokeTestAScenario)constructor.Invoke(constructorParameters);
                    return instance;
                }
                catch (Exception)
                {
                    continue;
                }
            }

            // We couldn't use any of its Constructor. Let's return a default instance (degraded mode)
            // write an error message
            return null;
        }

        private static object?[]? PrepareParametersForThisConstructor(ConstructorInfo constructor, IServiceProvider serviceProvider)
        {
            var parameters = new List<object>();
            var parameterInfos = constructor.GetParameters();
            foreach (var parameterInfo in parameterInfos)
            {
                var type = parameterInfo.ParameterType;

                // Default .NET types
                parameters.Add(serviceProvider.GetService(type));
            }

            return parameters.ToArray();
        }

        private static Type[] GetTypesImplementing<T>()
        {
            var smokeTesTypes = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes()
                                                .Where(p => typeof(T).IsAssignableFrom(p))
                                                .ToArray();
                    smokeTesTypes.AddRange(types);
                }
                catch (Exception e)
                {
                    // something went wrong during the reflection phase
                }
            }

            
            return smokeTesTypes.ToArray();
        }
    }
}