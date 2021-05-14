using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Sample.ExternalSmokeTests.Utilities;
using SmokeMe;

namespace Sample.ExternalSmokeTests
{
    [Category("DB")]
    [Category("Booking")]
    public class BookingSmokeTest : SmokeTest
    {
        private readonly IToggleFeatures _featureToggles;
        private readonly IConfiguration _configuration;
        public override string SmokeTestName => "Booking smoke test";
        public override string Description => "Booking smoke test for testing purpose";

        public BookingSmokeTest(IToggleFeatures featureToggles, IConfiguration configuration)
        {
            _featureToggles = featureToggles;
            _configuration = configuration;
        }

        public override async Task<SmokeTestResult> Scenario()
        {
            if (_featureToggles.IsEnabled("mustTimeOut"))
            {
                var timeoutInMsec = int.Parse(_configuration[Constants.GlobaltimeoutinmsecConfigurationKey]);

                await Task.Delay(TimeSpan.FromMilliseconds(timeoutInMsec + 100));
            }

            return new SmokeTestResult(true);
        }
    }
}