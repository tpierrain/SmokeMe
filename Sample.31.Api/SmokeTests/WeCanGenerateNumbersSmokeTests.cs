using Sample.Api.FakeDomain;
using SmokeMe;

namespace Sample.Api.SmokeTests
{
    public class WeCanGenerateNumbersSmokeTests : ISmokeTestAScenario
    {
        private readonly IProviderNumbers _numbersProvider;

        public WeCanGenerateNumbersSmokeTests(IProviderNumbers numbersProvider)
        {
            _numbersProvider = numbersProvider;
        }

        public SmokeTestResult ExecuteScenario()
        {
            _numbersProvider.GiveMeANumber();

            return new SmokeTestResult(true);
        }
    }
}
