using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NFluent;
using NSubstitute;
using NUnit.Framework;
using Smoke.Controllers;
using Smoke.Tests.Helpers;

namespace Smoke.Tests
{
    [TestFixture]
    public class SmokeControllerShould
    {
        [Test]
        public async Task Run_all_smoke_tests()
        {
            var configuration = Substitute.For<IConfiguration>();
            var smokeTestProvider = Stub.ASmokeTestProvider(new AlwaysPositiveSmokeTest(TimeSpan.Zero), new SmokeTestThrowingAnAccessViolationException(TimeSpan.Zero));

            var controller = new SmokeController(configuration, null, smokeTestProvider);
            var smokeTestResult = await controller.RunSmokeTests();

            Check.That(smokeTestResult.Results.Select(x => x.Outcome)).ContainsExactly(true, false);
        }

        [Test]
        public void Return_false_success_when_smoke_tests_timeout_globally()
        {
            var globalTimeoutInMsec = 2000;
            var configuration = Stub.AConfiguration(globalTimeoutInMsec);
            var smokeTestProvider = Stub.ASmokeTestProvider(new AlwaysPositiveSmokeTest(TimeSpan.FromSeconds(1.1)), new SmokeTestThrowingAnAccessViolationException(TimeSpan.FromSeconds(3.0)));

            var controller = new SmokeController(configuration, null, smokeTestProvider);

            SmokeTestSessionResult smokeTestResult = null;

            var acceptableDeltaInMsec = 700;
            Check.ThatAsyncCode(async () =>
            {
                smokeTestResult = await controller.RunSmokeTests();
            }).LastsLessThan(globalTimeoutInMsec + acceptableDeltaInMsec, TimeUnit.Milliseconds);

            Check.That(smokeTestResult.IsSuccess).IsFalse();
        }

        [Test]
        public void Only_accept_null_ServiceProvider_for_unit_testing_purpose_when_we_provide_a_non_null_SmokeTestProvider()
        {
            var smokeControllerForTesting = new SmokeController(Substitute.For<IConfiguration>(), null, Substitute.For<IFindSmokeTests>());

            Check.ThatCode(() =>
            {
                var smokeControllerForRealUsage = new SmokeController(Substitute.For<IConfiguration>(), null, null);
            }).Throws<ArgumentNullException>().WithMessage("Must provide a non-null serviceProvider when smokeTestProvider is not provided. (Parameter 'serviceProvider')");
        }
    }
}