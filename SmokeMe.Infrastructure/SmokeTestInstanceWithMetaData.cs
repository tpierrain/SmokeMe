namespace SmokeMe
{
    public class SmokeTestInstanceWithMetaData
    {
        public int? SmokeTestIdentifier { get; set; }
        public SmokeTest SmokeTest { get; }
        public string[] Categories { get; }

        public SmokeTestInstanceWithMetaData(SmokeTest smokeTest, params string[] categories)
        {
            SmokeTest = smokeTest;
            Categories = categories;
        }
    }
}