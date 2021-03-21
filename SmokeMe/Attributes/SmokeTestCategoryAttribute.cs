using System;

namespace SmokeMe
{
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