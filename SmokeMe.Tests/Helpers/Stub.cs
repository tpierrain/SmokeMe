using System;
using Diverse;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Sample.Api.FakeDomain;

namespace SmokeMe.Tests.Helpers
{
    /// <summary>
    /// Helper class to easily stub dependencies.
    /// </summary>
    public class Stub
    {
        public static IFindSmokeTests ASmokeTestProvider(params ISmokeTestAScenario[] smokeTestsToFind)
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