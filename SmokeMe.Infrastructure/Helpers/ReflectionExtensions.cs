using System.Reflection;

namespace SmokeMe.Helpers
{
    /// <summary>
    /// Extension methods related to the usage of Reflection.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets a value indicating whether a given <see cref="Type"/> is <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check.</param>
        /// <returns><b>true</b> if the <see cref="Type"/> is a <see cref="IEnumerable{T}"/> instance, <b>false</b> otherwise.</returns>
        public static bool IsEnumerable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        /// <summary>
        /// Gets all the constructors of a <see cref="Type"/> ordered by their number of parameters desc.
        /// </summary>
        /// <param name="type">The considered <see cref="Type"/>.</param>
        /// <returns>All the constructors of a <see cref="Type"/> ordered by their number of parameters desc.</returns>
        public static IEnumerable<ConstructorInfo> GetConstructorsOrderedByNumberOfParametersDesc(this Type type)
        {
            var constructors = ((System.Reflection.TypeInfo)type).DeclaredConstructors;

            return constructors.OrderByDescending(c => c.GetParameters().Length);
        }
    }
}