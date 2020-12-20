using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NFluent;
using NSubstitute;
using NUnit.Framework;
using SmokeMe.Controllers;
using SmokeMe.Tests.Helpers;
using SmokeMe.Tests.Unit;

namespace SmokeMe.Tests.Acceptance
{
    [TestFixture]
    public class SmokeControllerShould
    {
        private readonly SomeSmokeTestsForTestingPurposeShould _someSmokeTestsForTestingPurposeShould = new SomeSmokeTestsForTestingPurposeShould();

        [Test]
        [Repeat(10)]
        public async Task Run_all_smoke_tests()
        {
            var configuration = Substitute.For<IConfiguration>();
            var smokeTestProvider = Stub.ASmokeTestProvider(new AlwaysPositiveSmokeTest(TimeSpan.Zero), new SmokeTestThrowingAnAccessViolationException(TimeSpan.Zero));

            var controller = new SmokeController(configuration, null, smokeTestProvider);
            var response = await controller.ExecuteSmokeTests();

            var smokeTestResult = response.CheckIsError<SmokeTestSessionResultDto>(HttpStatusCode.InternalServerError);
            Check.That(smokeTestResult.Results.Select(x => x.Outcome)).Contains(true, false);
        }

        [Test]
        public async Task Return_false_success_when_smoke_tests_timeout_globally()
        {
            var globalTimeoutInMsec = 1000;
            var configuration = Stub.AConfiguration(globalTimeoutInMsec);
            var smokeTestProvider = Stub.ASmokeTestProvider(new AlwaysPositiveSmokeTest(TimeSpan.FromSeconds(1.1)), new SmokeTestThrowingAnAccessViolationException(TimeSpan.FromSeconds(1.0)));

            var controller = new SmokeController(configuration, null, smokeTestProvider);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var response = await controller.ExecuteSmokeTests();
            stopwatch.Stop();

            var smokeTestResult = response.CheckIsError<SmokeTestSessionResultDto>(HttpStatusCode.InternalServerError);

            var acceptableDeltaInMsec = 700; // delta to make this test less fragile with exotic or lame CI agents
            Check.That(stopwatch.Elapsed).IsLessThan(TimeSpan.FromMilliseconds(globalTimeoutInMsec + acceptableDeltaInMsec));
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

        [Test]
        public async Task Return_execution_durations_in_readable_and_adjusted_string_format()
        {
            var smokeTestProvider = Stub.ASmokeTestProvider(new AlwaysPositiveSmokeTest(TimeSpan.FromSeconds(1)), new AlwaysPositiveSmokeTest(TimeSpan.FromMilliseconds(30)), new AlwaysPositiveSmokeTest(TimeSpan.FromSeconds(1.2)));
            var smokeController = new SmokeController(Substitute.For<IConfiguration>(), null, smokeTestProvider);

            var response = await smokeController.ExecuteSmokeTests();

            response.CheckIsOk200();
            var smokeTestResult = response.ExtractValue<SmokeTestSessionResultDto>();

            Check.That(smokeTestResult.Results[0].Duration).IsEqualTo("1 second");
            Check.That(smokeTestResult.Results[1].Duration).Contains(" milliseconds");
            Check.That(smokeTestResult.Results[2].Duration).Contains(" seconds");
        }
    }
}