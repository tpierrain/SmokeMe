using System;
using Diverse;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Sample.Api.FakeDomain;
using Sample.ExternalSmokeTests.Utilities;

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

        private static IServiceProvider FeatureToggles(params FeatureToggle[] featureToggles)
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var toggleFeatures = Substitute.For<IToggleFeatures>();
            foreach (var featureToggle in featureToggles)
            {
                toggleFeatures.IsEnabled(featureToggle.FeatureName).Returns(featureToggle.FeatureValue);
            }
            
            serviceProvider.GetService(typeof(IToggleFeatures)).Returns(toggleFeatures);
            return serviceProvider;
        }

        public static IServiceProvider ACompleteServiceProvider(IConfiguration configuration, params FeatureToggle[] featureToggles)
        {
            var aCompleteServiceProvider = Stub.FeatureToggles(featureToggles);

            aCompleteServiceProvider.GetService(typeof(IConfiguration)).Returns(configuration);

            return aCompleteServiceProvider;
        }
    }
}