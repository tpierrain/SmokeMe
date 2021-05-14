using System;
using System.Threading.Tasks;
using Diverse;
using Microsoft.Extensions.Configuration;
using Sample.Api.FakeDomain;
using SmokeMe;
using SmokeMe.Helpers;

namespace Sample.Api.SmokeTests
{
    public class FlippingSmokeTest : ICheckSmoke
    {
        private readonly IProviderNumbers _numbersProvider;
        private readonly IFuzz _fuzzer;
        private readonly IConfiguration _configuration;
        public string SmokeTestName => "Flipping smoke test";
        public string Description => $"For unit testing purpose. Smoke test being able to randomly timeout, succeeded or failed.";

        /// <summary>
        /// Gets a value indicating whether or not this smoke test must be discarded (may be interesting to coupled with feature toggle mechanism).
        /// </summary>
        public bool MustBeDiscarded => false;

        public FlippingSmokeTest(IProviderNumbers numbersProvider, IFuzz fuzzer, IConfiguration configuration)
        {
            _numbersProvider = numbersProvider;
            _fuzzer = fuzzer;
            _configuration = configuration;
        }

        public async Task<SmokeTestResult> Scenario()
        {
            var delayInMsec = _fuzzer.GenerateInteger(100, 1700);

            if (_fuzzer.HeadsOrTails())
            {
                // force a timeout
                delayInMsec = Convert.ToInt32(_configuration.GetSmokeMeGlobalTimeout().Add(TimeSpan.FromSeconds(1)).TotalMilliseconds);
            }

            await Task.Delay(delayInMsec);

            _numbersProvider.GiveMeANumber();

            if (_fuzzer.HeadsOrTails())
            {
                return new SmokeTestResult(true);
            }
            else
            {
                return new SmokeTestResult(false);
            }
        }
    }
}
