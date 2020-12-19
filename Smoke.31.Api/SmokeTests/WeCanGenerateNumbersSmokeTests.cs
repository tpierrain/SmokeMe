using Smoke.Api.FakeDomain;

namespace Smoke.Api.SmokeTests
{
    public class WeCanGenerateNumbersSmokeTests : ITestWithSmoke
    {
        private readonly IProviderNumbers _numbersProvider;

        public WeCanGenerateNumbersSmokeTests(IProviderNumbers numbersProvider)
        {
            _numbersProvider = numbersProvider;
        }

        public SmokeTestResult Run()
        {
            _numbersProvider.GiveMeANumber();

            return new SmokeTestResult(true);
        }
    }
}
