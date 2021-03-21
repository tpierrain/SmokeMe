namespace SmokeMe
{
    public class SmokeTestInstanceWithMetaData
    {
        public int? SmokeTestIdentifier { get; set; }
        public ICheckSmoke SmokeTest { get; }
        public string[] Categories { get; }

        public SmokeTestInstanceWithMetaData(ICheckSmoke smokeTest, params string[] categories)
        {
            SmokeTest = smokeTest;
            Categories = categories;
        }
    }
}