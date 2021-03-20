using System;

namespace SmokeMe.Attributes
{
    public class SmokeTestCategoryAttribute : Attribute
    {
        public string CategoryName { get; }

        public SmokeTestCategoryAttribute(string categoryName)
        {
            CategoryName = categoryName;
        }
    }
}