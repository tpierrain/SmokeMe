using System;
using System.Linq;
using Diverse;
using NFluent;
using NUnit.Framework;
using Smoke.Api.SmokeTests;
using Smoke.Tests.Helpers;

namespace Smoke.Tests
{
    [TestFixture]
    public class SmokeTestAutoFinderShould
    {
        [Test]
        [Ignore("TBF")]
        public void Instantiate_all_concrete_classes_implementing_ITestWithSmoke()
        {
            var serviceProvider = Stub.AServiceProvider();

            var smokeTestAutoFinder = new SmokeTestAutoFinder(serviceProvider);

            var smokeTests = smokeTestAutoFinder.FindAllSmokeTestsToRun();

            Check.That(smokeTests.Select(x => x.GetType()))
                .Contains(typeof(AlwaysPositiveSmokeTest), typeof(SmokeTestThrowingAnAccessViolationException), typeof(WeCanGenerateNumbersSmokeTests));
        }
    }
}