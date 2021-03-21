using System;

namespace SmokeMe
{
    /// <summary>
    /// Allows to associate a Category for a Smoke Test (i.e.: a <see cref="ICheckSmoke"/> type).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SmokeTestCategoryAttribute : Attribute
    {
        public string CategoryName { get; }

        public SmokeTestCategoryAttribute(string categoryName)
        {
            CategoryName = categoryName;
        }
    }
}