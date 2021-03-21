using System;

namespace SmokeMe
{
    /// <summary>
    /// Allows to associate a Category for a Smoke Test (i.e.: a <see cref="ICheckSmoke"/> type).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CategoryAttribute : Attribute
    {
        public string CategoryName { get; }

        public CategoryAttribute(string categoryName)
        {
            CategoryName = categoryName;
        }
    }
}