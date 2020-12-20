using System.Threading.Tasks;
using Sample.Api.FakeDomain;
using SmokeMe;

namespace Sample.Api.SmokeTests
{
    public class WeCanGenerateNumbersSmokeTests : ISmokeTestAScenario
    {
        private readonly IProviderNumbers _numbersProvider;
        public string SmokeTestName => "Check Number provider availability.";
        public string Description => $"For unit testing purpose. Check that an external service (here {nameof(IProviderNumbers)}) may be injected properly to a smoke test.";

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
