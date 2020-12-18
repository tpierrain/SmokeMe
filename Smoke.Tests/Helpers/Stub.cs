using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace Smoke.Tests.Helpers
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
    }
}