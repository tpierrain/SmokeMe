namespace SmokeMe.Tests.Helpers
{
    public static class SmokeTestExtensions
    {
        public static SmokeTestInstanceWithMetaData WithAssociatedCategories(this ICheckSmoke smokeTest, params string[] categories)
        {
            return new SmokeTestInstanceWithMetaData(smokeTest, categories);
        }

        public static SmokeTestInstanceWithMetaData WithoutCategory(this ICheckSmoke smokeTest)
        {
            return new SmokeTestInstanceWithMetaData(smokeTest, new string[0]);
        }
    }
}