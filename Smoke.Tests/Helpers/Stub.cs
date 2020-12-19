using System;
using Diverse;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Smoke.Api.FakeDomain;

namespace Smoke.Tests.Helpers
{
    /// <summary>
    /// Helper class to easily stub dependencies.
    /// </summary>
    public class Stub
    {
        public static IFindSmokeTests ASmokeTestProvider(params ITestWithSmoke[] smokeTestsToFind)
        {
            var smokeTestProvider = Substitute.For<IFindSmokeTests>();
            smokeTestProvider.FindAllSmokeTestsToRun().Returns(smokeTestsToFind);
            return smokeTestProvider;
        }

        public static IConfiguration AConfiguration(int globalTimeoutInMsec)
        {
            var configuration = Substitute.For<IConfiguration>();
            configuration[Constants.GlobaltimeoutinmsecConfigurationKey].Returns(globalTimeoutInMsec.ToString());

            return configuration;
        }

        public static IServiceProvider AServiceProvider()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();

            serviceProvider.GetService(typeof(IProviderNumbers)).Returns(new NumberProvider(new Fuzzer()));
            return serviceProvider;
        }
    }
}