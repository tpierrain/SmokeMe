using Diverse;

namespace Sample.Api.FakeDomain
{
    public class NumberProvider : IProviderNumbers
    {
        private readonly IFuzz _fuzzer;

        public NumberProvider(IFuzz fuzzer)
        {
            _fuzzer = fuzzer;
        }

        public int GiveMeANumber()
        {
            return _fuzzer.GenerateInteger();
        }
    }

    public interface IProviderNumbers
    {
        int GiveMeANumber();
    }
}
