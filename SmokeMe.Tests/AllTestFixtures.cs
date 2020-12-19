using Diverse;
using NUnit.Framework;

namespace SmokeMe.Tests
{
    [SetUpFixture]
    public class AllTestFixtures
    {
        [OneTimeSetUp]
        public void Init()
        {
            Fuzzer.Log = TestContext.WriteLine;
        }
    }
}