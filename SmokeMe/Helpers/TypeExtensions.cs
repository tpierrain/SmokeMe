using System;
using System.Collections.Generic;
using System.Linq;

namespace SmokeMe.Helpers
{
    /// <summary>
    /// Extension methods for <see cref="Type"/>.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Indicates whether or not a considered type has the Ignore custom Attribute.
        /// </summary>
        /// <param name="consideredType">The type we want to check.</param>
        /// <returns><b>true</b> if the Type has an [Ignore()] attribute; <b>false</b> otherwise.</returns>
        public static bool HasIgnoredCustomAttribute(this Type consideredType)
        {
            var customAttributes = consideredType.CustomAttributes;

            foreach (var customAttributeData in customAttributes)
            {
                if (customAttributeData.AttributeType == typeof(IgnoreAttribute))
                {
                    return true;
                }
            }

            return false;
        }

        public static string[] GetCategories(this SmokeTest smokeTest)
        {
            return GetCategories(smokeTest.GetType());
        }

        public static string[] GetCategories(Type smokeTestType)
        {
            var categories = smokeTestType.CustomAttributes.Where(c => c.AttributeType == typeof(CategoryAttribute)).Select(t => t.ConstructorArguments[0].Value).Cast<string>().ToArray();
            
            return categories;
        }

        public static Type[] RemoveSmokeTestsWithIgnoredAttribute(this Type[] types)
        {
            var ignoredTypes = types.Where(t => t.HasIgnoredCustomAttribute()).ToArray();

            types = types.Except(ignoredTypes).ToArray();
            return types;
        }

        public static IEnumerable<Type> GetIgnoredSmokeTests(this IEnumerable<Type> types)
        {
            var ignoredTypes = types.Where(t => t.HasIgnoredCustomAttribute()).ToArray();

            return ignoredTypes.ToArray();
        }
    }
}