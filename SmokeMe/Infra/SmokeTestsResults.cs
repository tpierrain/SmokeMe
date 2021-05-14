using System.Linq;

namespace SmokeMe.Infra
{
    public class SmokeTestsResults
    {
        private readonly SmokeTestResultWithMetaDataDto[] _allResults;

        public int NbOfTestsRan => _allResults.Length;

        public int NbOfFailures => Failures.Length;

        public int NbOfSuccesses => Successes.Length;

        public SmokeTestResultWithMetaDataDto[] Failures => _allResults.Where(x => x.Outcome == false).ToArray();
        public SmokeTestResultWithMetaDataDto[] Successes => _allResults.Where(x => x.Outcome == true).ToArray();

        public SmokeTestsResults(SmokeTestResultWithMetaDataDto[] allResults)
        {
            _allResults = allResults;
        }
    }
}