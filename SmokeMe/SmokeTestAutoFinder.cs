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
        /// <param name="serviceProvider">The (IoC) <see cref="IServiceProvider"/> instance needed to instantiate <see cref="SmokeTest"/> instances.</param>
        public SmokeTestAutoFinder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Finds all smoke tests scenarii that have to be executed for this API.
        /// </summary>
        /// <param name="requestedCategories">Categories requested by the client.</param>
        /// <returns>The collection of all <see cref="SmokeTest"/> instance declared in this API to be executed.</returns>
        public IEnumerable<SmokeTestInstanceWithMetaData> FindAllSmokeTestsToRun(params string[] requestedCategories)
        {
            var smokeTestInstances = new List<SmokeTestInstanceWithMetaData>();

            var types = GetTypesImplementing<SmokeTest>(requestedCategories);

            types = RemoveSmokeTestsWithIgnoredAttribute(types);

            int smokeTestIdentifier = 1;
            foreach (var smokeTestType in types)
            {
                var constructors = smokeTestType.GetConstructorsOrderedByNumberOfParametersDesc();
                var smokeTest = InstantiateSmokeTest(constructors, _serviceProvider);
                var smokeTestCategories = GetCategories(smokeTestType);
                if (smokeTest != null)
                {
                    var smokeTestInstanceWithMetaData = new SmokeTestInstanceWithMetaData(smokeTest, smokeTestCategories) { SmokeTestIdentifier = smokeTestIdentifier };

                    smokeTestInstances.Add(smokeTestInstanceWithMetaData);
                }

                smokeTestIdentifier++;
            }
            
            return smokeTestInstances;
        }

        private string[] GetCategories(Type smokeTestType)
        {
            var categories = smokeTestType.CustomAttributes.Where(c => c.AttributeType == typeof(CategoryAttribute)).Select(t => t.ConstructorArguments[0].Value).Cast<string>().ToArray();
            
            return categories;
        }

        private static Type[] RemoveSmokeTestsWithIgnoredAttribute(Type[] types)
        {
            var ignoredTypes = types.Where(t => HasIgnoredCustomAttribute(t.CustomAttributes)).ToArray();

            types = types.Except(ignoredTypes).ToArray();
            return types;
        }

        private static bool HasIgnoredCustomAttribute(IEnumerable<CustomAttributeData> customAttributes)
        {
            foreach (var customAttributeData in customAttributes)
            {
                if (customAttributeData.AttributeType == typeof(IgnoreAttribute))
                {
                    return true;
                }
            }

            return false;
        }

        private static SmokeTest InstantiateSmokeTest(IEnumerable<ConstructorInfo> constructors, IServiceProvider serviceProvider)
        {
            foreach (var constructor in constructors)
            {
                try
                {
                    var constructorParameters = PrepareParametersForThisConstructor(constructor, serviceProvider);
                    var instance = (SmokeTest)constructor.Invoke(constructorParameters);
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

        private static object[] PrepareParametersForThisConstructor(ConstructorInfo constructor, IServiceProvider serviceProvider)
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

        private static Type[] GetTypesImplementing<T>(params string[] categories)
        {
            var smokeTesTypes = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes().Where(p => typeof(T).IsAssignableFrom(p)).ToArray();

                    if (types.Any())
                    {
                        if (categories.Length > 0)
                        {
                            // must filter types
                            types = types.Where(t => ContainsCategoryAttributes(categories, t.CustomAttributes)).ToArray();
                        }

                        smokeTesTypes.AddRange(types.ToArray());
                    }
                }
                catch (Exception)
                {
                    // something went wrong during the reflection phase
                }
            }
            
            return smokeTesTypes.ToArray();
        }

        private static bool ContainsCategoryAttributes(string[] categories, IEnumerable<CustomAttributeData> customAttributes)
        {
            if (customAttributes == null)
            {
                return false;
            }

            foreach (var customAttributeData in customAttributes)
            {
                if (customAttributeData.AttributeType == typeof(CategoryAttribute))
                {
                    foreach (var category in categories)
                    {
                        if (string.Compare(customAttributeData.ConstructorArguments[0].Value.ToString(), category, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}