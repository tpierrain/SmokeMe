using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NFluent;
using NSubstitute;
using NUnit.Framework;
using Sample.ExternalSmokeTests;
using SmokeMe.Controllers;
using SmokeMe.Infra;
using SmokeMe.Tests.Helpers;

namespace SmokeMe.Tests.Acceptance
{
    [TestFixture]
    public class SmokeControllerShould
    {
        [SetUp]
        public void SetUp()
        {
            ForceTheLoadingOfTheSampleExternalSmokeTestsAssembly();
        }

        [Test]
        [Repeat(10)]
        public async Task Run_all_smoke_tests()
        {
            var configuration = Substitute.For<IConfiguration>();
            var smokeTestProvider = Stub.ASmokeTestProvider(new AlwaysPositiveSmokeTest(TimeSpan.Zero).WithoutCategory(), new SmokeTestThrowingAnAccessViolationException(TimeSpan.Zero).WithoutCategory());

            var controller = new SmokeController(configuration, null, smokeTestProvider);
            var response = await controller.ExecuteSmokeTests();

            var reportDto = response.CheckIsError<SmokeTestsSessionReportDto>(HttpStatusCode.InternalServerError);
            Check.That(reportDto.Results.Select(x => x.Outcome)).Contains(true, false);
        }

        [Test]
        public async Task Return_InternalServerError_500_when_smoke_tests_fails()
        {
            var configuration = Substitute.For<IConfiguration>();
            var smokeTestProvider = Stub.ASmokeTestProvider(new SmokeTestThrowingAnAccessViolationException(TimeSpan.Zero).WithoutCategory());

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
            var smokeTestProvider = Stub.ASmokeTestProvider(new AlwaysPositiveSmokeTest(TimeSpan.FromSeconds(6)).WithoutCategory(), new AlwaysPositiveSmokeTest(TimeSpan.FromSeconds(2.0)).WithoutCategory());

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
            var smokeTestProvider = Stub.ASmokeTestProvider(new AlwaysPositiveSmokeTest(TimeSpan.FromSeconds(1)).WithoutCategory(), new AlwaysPositiveSmokeTest(TimeSpan.FromMilliseconds(30)).WithoutCategory(), new AlwaysPositiveSmokeTest(TimeSpan.FromSeconds(1.2)).WithoutCategory());
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
            Check.That(reportDto.Status).IsEqualTo($"No smoke test have been found in your executing assemblies. Start adding (not ignored) {nameof(ICheckSmoke)} types in your code base so that the SmokeMe library can detect and run them.");
        }

        [Test]
        public async Task Return_503_Service_Unavailable_when_SmokeMe_is_disabled_in_configuration()
        {
            var configuration = Stub.AConfiguration(false);
            
            var smokeTestProvider = Stub.ASmokeTestProvider(new AlwaysPositiveSmokeTest(TimeSpan.Zero).WithAssociatedCategories("DB"), new SmokeTestThrowingAnAccessViolationException(TimeSpan.Zero).WithoutCategory());

            var controller = new SmokeController(configuration, null, smokeTestProvider);
            var response = await controller.ExecuteSmokeTests();

            var reportDto = response.CheckIsError<SmokeTestsSessionReportDto>(HttpStatusCode.ServiceUnavailable);
            Check.That(reportDto.Results).IsEmpty();
            Check.That(reportDto.IsSuccess).IsFalse();
            Check.That(reportDto.Status).IsEqualTo($"Smoke tests execution not enabled. Set the '{Constants.IsEnabledConfigurationKey}' configuration key to true if you want to enable it.");
        }

        [Test]
        public async Task Only_Execute_corresponding_SmokeTest_when_specifying_one_Category()
        {
            var configuration = Stub.AConfiguration(true);
            var serviceProvider = Substitute.For<IServiceProvider>();
            var smokeTestAutoFinder = new SmokeTestAutoFinder(serviceProvider);

            var controller = new SmokeController(configuration, serviceProvider, smokeTestAutoFinder);

            var response = await controller.ExecuteSmokeTests("DB");

            response.CheckIsOk200();
            var reportDto = response.ExtractValue<SmokeTestsSessionReportDto>();

            Check.That(reportDto.Results).HasSize(1);

            // Always positive smoke test after a delay
            Check.That(reportDto.Results[0].SmokeTestName).IsEqualTo("Always working DB smoke test");
            Check.That(reportDto.Results[0].Outcome).IsTrue();
        }

        [Test]
        public async Task Only_Execute_SmokeTest_with_Specified_Categories()
        {
            var configuration = Stub.AConfiguration(true);
            var serviceProvider = Substitute.For<IServiceProvider>();
            var smokeTestAutoFinder = new SmokeTestAutoFinder(serviceProvider);

            var controller = new SmokeController(configuration, serviceProvider, smokeTestAutoFinder);

            var response = await controller.ExecuteSmokeTests("FailingSaMere", "DB");

            response.CheckIsError<SmokeTestsSessionReportDto>(HttpStatusCode.InternalServerError);
            var reportDto = response.ExtractValue<SmokeTestsSessionReportDto>();

            Check.That(reportDto.Results).HasSize(2);

            // Always positive smoke test after a delay
            Check.That(reportDto.Results[0].SmokeTestName).IsEqualTo("Failing on purpose");
            Check.That(reportDto.Results[0].Outcome).IsFalse();

            Check.That(reportDto.Results[1].SmokeTestName).IsEqualTo("Always working DB smoke test");
            Check.That(reportDto.Results[1].Outcome).IsTrue();
        }

        [Test]
        public async Task Return_NotImplemented_501_with_proper_didactic_message_when_specifying_undeclared_Category()
        {
            var configuration = Stub.AConfiguration(true);
            var serviceProvider = Substitute.For<IServiceProvider>();
            var smokeTestAutoFinder = new SmokeTestAutoFinder(serviceProvider);

            var controller = new SmokeController(configuration, serviceProvider, smokeTestAutoFinder);

            var nonExistingCategoryName = "PortnaouaqThisIsNotAnExistingCategory";
            var response = await controller.ExecuteSmokeTests(nonExistingCategoryName);

            response.CheckIsError<SmokeTestsSessionReportDto>(HttpStatusCode.NotImplemented);
            var reportDto = response.ExtractValue<SmokeTestsSessionReportDto>();

            Check.That(reportDto.Results).HasSize(0);
            Check.That(reportDto.Status).IsEqualTo(@$"No smoke test with [Category(""{nonExistingCategoryName}"")] attribute have been found in your executing assemblies. Check that you have one or more (not ignored) ICheckSmoke types in your code base with the declared attribute [Category(""{nonExistingCategoryName}"")] so that the SmokeMe library can detect and run them.");
        }

        [Test]
        public async Task Return_NotImplemented_501_with_proper_didactic_message_when_specifying_multiple_undeclared_Categories()
        {
            var configuration = Stub.AConfiguration(true);
            var serviceProvider = Substitute.For<IServiceProvider>();
            var smokeTestAutoFinder = new SmokeTestAutoFinder(serviceProvider);

            var controller = new SmokeController(configuration, serviceProvider, smokeTestAutoFinder);

            var response = await controller.ExecuteSmokeTests("Cat1", "Cat2", "Cat3");

            response.CheckIsError<SmokeTestsSessionReportDto>(HttpStatusCode.NotImplemented);
            var reportDto = response.ExtractValue<SmokeTestsSessionReportDto>();

            Check.That(reportDto.Results).HasSize(0);
            Check.That(reportDto.Status).IsEqualTo(@$"No smoke test with [Category(""Cat1"")] or [Category(""Cat2"")] or [Category(""Cat3"")] attributes have been found in your executing assemblies. Check that you have one or more (not ignored) ICheckSmoke types in your code base with the expected declared [Category] attributes so that the SmokeMe library can detect and run them.");
        }

        [Test]
        public async Task Not_run_SmokeTests_with_Ignore_Attribute()
        {
            var configuration = Stub.AConfiguration(true);
            var serviceProvider = Substitute.For<IServiceProvider>();
            var smokeTestAutoFinder = new SmokeTestAutoFinder(serviceProvider);

            var controller = new SmokeController(configuration, serviceProvider, smokeTestAutoFinder);

            var response = await controller.ExecuteSmokeTests("Awkward");

            response.CheckIsError<SmokeTestsSessionReportDto>(HttpStatusCode.NotImplemented);
            var reportDto = response.ExtractValue<SmokeTestsSessionReportDto>();

            Check.That(reportDto.Results).HasSize(0);
            Check.That(reportDto.Status).IsEqualTo(@$"No smoke test with [Category(""Awkward"")] attribute have been found in your executing assemblies. Check that you have one or more (not ignored) ICheckSmoke types in your code base with the declared attribute [Category(""Awkward"")] so that the SmokeMe library can detect and run them.");
        }

        [Test]
        public async Task Publish_the_executed_SmokeTestCategories_when_specified_by_the_client()
        {
            var configuration = Stub.AConfiguration(true);
            var serviceProvider = Substitute.For<IServiceProvider>();
            var smokeTestAutoFinder = new SmokeTestAutoFinder(serviceProvider);

            var controller = new SmokeController(configuration, serviceProvider, smokeTestAutoFinder);

            var response = await controller.ExecuteSmokeTests("FailingSaMere", "DB");

            response.CheckIsError<SmokeTestsSessionReportDto>(HttpStatusCode.InternalServerError);
            var reportDto = response.ExtractValue<SmokeTestsSessionReportDto>();

            Check.That(reportDto.Results).HasSize(2);
            Check.That(reportDto.RequestedCategories).Contains("FailingSaMere", "DB");
        }

        [Test]
        public async Task Publish_the_Categories_of_every_executed_SmokeTest_when_existing()
        {
            var configuration = Stub.AConfiguration(true);
            var serviceProvider = Substitute.For<IServiceProvider>();
            var smokeTestAutoFinder = new SmokeTestAutoFinder(serviceProvider);

            var controller = new SmokeController(configuration, serviceProvider, smokeTestAutoFinder);

            var response = await controller.ExecuteSmokeTests();

            response.CheckIsError<SmokeTestsSessionReportDto>(HttpStatusCode.InternalServerError);
            var reportDto = response.ExtractValue<SmokeTestsSessionReportDto>();

            Check.That(reportDto.Results).HasSize(5);
            Check.That(reportDto.RequestedCategories).IsEmpty();

            Check.That(reportDto.Results.Select(x => x.SmokeTestName)).ContainsExactly("Failing on purpose", "Always positive smoke test after a delay", "Throwing exception after a delay", "Always working DB smoke test", "Check connectivity towards Google search engine.");
            Check.That(reportDto.Results.Select(x => x.SmokeTestCategories)).ContainsExactly("FailingSaMere", "Tests", "", "DB, Booking", "Connectivity");
        }

        private static void ForceTheLoadingOfTheSampleExternalSmokeTestsAssembly()
        {
            new BookingSmokeTest();
        }
    }
}