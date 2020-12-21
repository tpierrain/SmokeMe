using System.Linq;
using NFluent;
using NUnit.Framework;
using Sample.Api.SmokeTests;
using SmokeMe.Tests.Helpers;

namespace SmokeMe.Tests.Unit
{
    [TestFixture]
    public class SmokeTestAutoFinderShould
    {
        [Test]
        public void Instantiate_all_concrete_classes_implementing_ITestWithSmoke()
        {
            var serviceProvider = Stub.AServiceProvider();

            var smokeTestAutoFinder = new SmokeTestAutoFinder(serviceProvider);

            var smokeTests = smokeTestAutoFinder.FindAllSmokeTestsToRun();

            Check.That(smokeTests.Select(x => x.GetType()))
                .Contains(typeof(AlwaysPositiveSmokeTest), 
                                                typeof(SmokeTestThrowingAnAccessViolationException), 
                                                typeof(FlippingSmokeTest));
        }
    }
}