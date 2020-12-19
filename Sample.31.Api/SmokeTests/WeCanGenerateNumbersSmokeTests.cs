using System.Threading.Tasks;
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

        public Task<SmokeTestResult> ExecuteScenario()
        {
            _numbersProvider.GiveMeANumber();

            return Task.FromResult(new SmokeTestResult(true));
        }
    }
}
