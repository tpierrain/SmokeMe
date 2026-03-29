using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NFluent;
using NSubstitute;
using NUnit.Framework;
using Sample.ExternalSmokeTests.Utilities;
using SmokeMe.AspNetCore;
using SmokeMe.Infra;
using SmokeMe.Tests.Helpers;
using SmokeMe.Tests.SmokeTests;

namespace SmokeMe.Tests.Acceptance
{
    [TestFixture]
    public class CleanUpShould
    {
        [Test]
        public async Task Be_called_after_a_successful_scenario()
        {
            var smokeTest = new SmokeTestWithSuccessfulCleanUp();
            var smokeTestProvider = Stub.ASmokeTestProvider(smokeTest.WithoutCategory());
            var controller = new SmokeController(Stub.ASmokeTestConfiguration(), null, smokeTestProvider);

            var response = await controller.ExecuteSmokeTests();

            response.CheckIsOk200();
            var reportDto = response.ExtractValue<SmokeTestsSessionReportDto>();

            Check.That(reportDto.Results.Successes).HasSize(1);
            Check.That(reportDto.Results.Successes[0].Outcome).IsTrue();
            Check.That(reportDto.Results.Successes[0].CleanupError).IsNull();
            Check.That(smokeTest.CleanUpWasCalled).IsTrue();
        }

        [Test]
        public async Task Not_flip_the_outcome_when_cleanup_fails_after_a_successful_scenario()
        {
            var smokeTest = new SmokeTestWithFailingCleanUp(scenarioSucceeds: true);
            var smokeTestProvider = Stub.ASmokeTestProvider(smokeTest.WithoutCategory());
            var controller = new SmokeController(Stub.ASmokeTestConfiguration(), null, smokeTestProvider);

            var response = await controller.ExecuteSmokeTests();

            response.CheckIsOk200();
            var reportDto = response.ExtractValue<SmokeTestsSessionReportDto>();

            Check.That(reportDto.Results.Successes).HasSize(1);
            Check.That(reportDto.Results.Successes[0].Outcome).IsTrue();
            Check.That(reportDto.Results.Successes[0].CleanupError).IsNotNull();
            Check.That(reportDto.Results.Successes[0].CleanupError.Message).Contains("CleanUp failed");
        }

        [Test]
        public async Task Preserve_original_error_and_report_cleanup_error_when_both_fail()
        {
            var smokeTest = new SmokeTestWithFailingCleanUp(scenarioSucceeds: false);
            var smokeTestProvider = Stub.ASmokeTestProvider(smokeTest.WithoutCategory());
            var controller = new SmokeController(Stub.ASmokeTestConfiguration(), null, smokeTestProvider);

            var response = await controller.ExecuteSmokeTests();

            var reportDto = response.CheckIsError<SmokeTestsSessionReportDto>(HttpStatusCode.InternalServerError);

            Check.That(reportDto.Results.Failures).HasSize(1);
            Check.That(reportDto.Results.Failures[0].Outcome).IsFalse();
            Check.That(reportDto.Results.Failures[0].CleanupError).IsNotNull();
            Check.That(reportDto.Results.Failures[0].CleanupError.Message).Contains("CleanUp failed");
        }

        [Test]
        public async Task Not_be_called_when_smoke_test_is_discarded()
        {
            var featureToggles = Substitute.For<IToggleFeatures>();
            featureToggles.IsEnabled("featureToggledSmokeTest").Returns(false);

            var smokeTest = new FeatureToggledAlwaysPositiveSmokeTest(featureToggles, TimeSpan.FromMilliseconds(50));
            var smokeTestProvider = Stub.ASmokeTestProvider(smokeTest.WithoutCategory());
            var controller = new SmokeController(Stub.ASmokeTestConfiguration(), null, smokeTestProvider);

            var response = await controller.ExecuteSmokeTests();

            response.CheckIsOk200();
            var reportDto = response.ExtractValue<SmokeTestsSessionReportDto>();

            Check.That(reportDto.Results.Discards).HasSize(1);
            Check.That(reportDto.Results.Discards[0].CleanupError).IsNull();
        }

        [Test]
        public async Task Be_called_after_a_failing_scenario()
        {
            var smokeTest = new SmokeTestWithFailingCleanUp(scenarioSucceeds: false);
            var smokeTestProvider = Stub.ASmokeTestProvider(smokeTest.WithoutCategory());
            var controller = new SmokeController(Stub.ASmokeTestConfiguration(), null, smokeTestProvider);

            await controller.ExecuteSmokeTests();

            Check.That(smokeTest.CleanUpWasCalled).IsTrue();
        }
    }
}
