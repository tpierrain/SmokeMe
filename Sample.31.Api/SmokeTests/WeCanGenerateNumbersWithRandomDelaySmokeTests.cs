using System.Threading.Tasks;
using Diverse;
using Sample.Api.FakeDomain;
using SmokeMe;

namespace Sample.Api.SmokeTests
{
    public class WeCanGenerateNumbersWithRandomDelaySmokeTests : ICheckSmoke
    {
        private readonly IProviderNumbers _numbersProvider;
        private readonly IFuzz _fuzzer;
        public string SmokeTestName => "Check Number provider availability.";
        public string Description => $"For unit testing purpose. Check that an external service (here {nameof(IProviderNumbers)}) is properly injected during a smoke test execution.";

        public WeCanGenerateNumbersWithRandomDelaySmokeTests(IProviderNumbers numbersProvider, IFuzz fuzzer)
        {
            _numbersProvider = numbersProvider;
            _fuzzer = fuzzer;
        }

        public Task<SmokeTestResult> Scenario()
        {
            var delayInMsec = _fuzzer.GenerateInteger(100, 1700);

            var continuation = Task.Delay(delayInMsec).ContinueWith(t =>
            {
                _numbersProvider.GiveMeANumber();
                return new SmokeTestResult(true);
            });

            return continuation;
        }
    }
}
