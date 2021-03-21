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
        public static IFindSmokeTests ASmokeTestProvider(params SmokeTestInstanceWithMetaData[] smokeTestsToFind)
        {
            var smokeTestProvider = Substitute.For<IFindSmokeTests>();

            var identifier = 1;
            foreach (var smokeTestInstanceWithMetaData in smokeTestsToFind)
            {
                smokeTestInstanceWithMetaData.SmokeTestIdentifier = identifier;
                identifier++;
            }

            smokeTestProvider.FindAllSmokeTestsToRun().Returns(smokeTestsToFind);
            return smokeTestProvider;
        }

        public static IConfiguration AConfiguration(bool? isEnabled = null, int? globalTimeoutInMsec = null)
        {
            var configuration = Substitute.For<IConfiguration>();
            if (globalTimeoutInMsec.HasValue)
            {
                configuration[Constants.GlobaltimeoutinmsecConfigurationKey].Returns(globalTimeoutInMsec.ToString());
            }

            if (isEnabled.HasValue)
            {
                configuration[Constants.IsEnabledConfigurationKey].Returns(isEnabled.ToString());
            }

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