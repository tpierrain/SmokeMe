using Diverse;
using NUnit.Framework;

namespace Smoke.Tests
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