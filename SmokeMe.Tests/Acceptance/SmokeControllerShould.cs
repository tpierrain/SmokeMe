using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NFluent;
using NSubstitute;
using NUnit.Framework;
using SmokeMe.Controllers;
using SmokeMe.Infra;
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

            var reportDto = response.CheckIsError<SmokeTestsSessionReportDto>(HttpStatusCode.InternalServerError);
            Check.That(reportDto.Results.Select(x => x.Outcome)).Contains(true, false);
        }

        [Test]
        public async Task Return_InternalServerError_500_when_smoke_tests_fails()
        {
            var configuration = Substitute.For<IConfiguration>();
            var smokeTestProvider = Stub.ASmokeTestProvider(new SmokeTestThrowingAnAccessViolationException(TimeSpan.Zero));

            var controller = new SmokeController(configuration, null, smokeTestProvider);
            var response = await controller.ExecuteSmokeTests();

            var reportDto = response.CheckIsError<SmokeTestsSessionReportDto>(HttpStatusCode.InternalServerError);
            Check.That(reportDto.Results.Select(x => x.Outcome)).ContainsExactly(false);
        }

        [Test]
        public async Task Return_GatewayTimeout_504_when_smoke_tests_timeout_but_provide_details()
        {
            var globalTimeoutInMsec = 5 * 1000;
            var configuration = Stub.AConfiguration(globalTimeoutInMsec: globalTimeoutInMsec);
            var smokeTestProvider = Stub.ASmokeTestProvider(new AlwaysPositiveSmokeTest(TimeSpan.FromSeconds(6)), new AlwaysPositiveSmokeTest(TimeSpan.FromSeconds(2.0)));

            var controller = new SmokeController(configuration, null, smokeTestProvider);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var response = await controller.ExecuteSmokeTests();
            stopwatch.Stop();

            var reportDto = response.CheckIsError<SmokeTestsSessionReportDto>(HttpStatusCode.GatewayTimeout);

            var acceptableDeltaInMsec = 1000; // delta to make this test less fragile with exotic or lame CI agents
            Check.That(stopwatch.Elapsed).IsLessThan(TimeSpan.FromMilliseconds(globalTimeoutInMsec + acceptableDeltaInMsec));
            Check.That(reportDto.IsSuccess).IsFalse();
            Check.That(reportDto.Status).IsEqualTo("One or more smoke tests have timeout (global timeout is: 5 seconds)");
            Check.That(reportDto.Results).HasSize(2);

            Check.That(reportDto.Results[0].Outcome).IsFalse();
            Check.That(reportDto.Results[0].Duration).IsEqualTo("timeout");
            Check.That(reportDto.Results[0].DurationInMsec).IsNull();

            Check.That(reportDto.Results[1].Outcome).IsTrue();
            Check.That(reportDto.Results[1].DurationInMsec.Value).IsLessThan(TimeSpan.FromSeconds(2.0).TotalMilliseconds + acceptableDeltaInMsec);
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
            var reportDto = response.ExtractValue<SmokeTestsSessionReportDto>();

            Check.That(reportDto.Results[0].Duration).IsEqualTo("1 second");
            Check.That(reportDto.Results[1].Duration).Contains(" milliseconds");
            Check.That(reportDto.Results[2].Duration).Contains(" seconds");
        }

        [Test]
        public async Task Return_501_error_code_not_implemented_when_no_smoke_test_is_found()
        {
            var smokeTestProvider = Stub.ASmokeTestProvider();
            var smokeController = new SmokeController(Substitute.For<IConfiguration>(), null, smokeTestProvider);

            var response = await smokeController.ExecuteSmokeTests();

            var reportDto = response.CheckIsError<SmokeTestsSessionReportDto>(HttpStatusCode.NotImplemented);

            Check.That(reportDto.IsSuccess).IsFalse();
            Check.That(reportDto.Results).IsEmpty();
            Check.That(reportDto.Status).IsEqualTo($"No smoke test have been found in your executing assemblies. Start adding {nameof(ICheckSmoke)} types in your code base so that the SmokeMe library can detect and run them.");
        }

        [Test]
        public async Task Return_503_Service_Unavailable_when_SmokeMe_is_disabled_in_configuration()
        {
            var configuration = Stub.AConfiguration(false);
            
            var smokeTestProvider = Stub.ASmokeTestProvider(new AlwaysPositiveSmokeTest(TimeSpan.Zero), new SmokeTestThrowingAnAccessViolationException(TimeSpan.Zero));

            var controller = new SmokeController(configuration, null, smokeTestProvider);
            var response = await controller.ExecuteSmokeTests();

            var reportDto = response.CheckIsError<SmokeTestsSessionReportDto>(HttpStatusCode.ServiceUnavailable);
            Check.That(reportDto.Results).IsEmpty();
            Check.That(reportDto.IsSuccess).IsFalse();
            Check.That(reportDto.Status).IsEqualTo($"Smoke tests execution not enabled. Set the '{Constants.IsEnabledConfigurationKey}' configuration key to true if you want to enable it.");
        }
    }
}