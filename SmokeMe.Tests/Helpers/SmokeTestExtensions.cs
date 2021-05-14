namespace SmokeMe.Tests.Helpers
{
    public static class SmokeTestExtensions
    {
        public static SmokeTestInstanceWithMetaData WithAssociatedCategories(this SmokeTest smokeTest, params string[] categories)
        {
            return new SmokeTestInstanceWithMetaData(smokeTest, categories);
        }

        public static SmokeTestInstanceWithMetaData WithoutCategory(this SmokeTest smokeTest)
        {
            return new SmokeTestInstanceWithMetaData(smokeTest, new string[0]);
        }
    }
}