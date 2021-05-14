using System.Linq;

namespace SmokeMe.Infra
{
    public class SmokeTestsResults
    {
        private readonly SmokeTestResultWithMetaDataDto[] _allResults;

        public int TotalOfTestsRan => _allResults.Length;

        public int NbOfFailures => Failures.Length;
        public int NbOfTimeouts => Timeouts.Length;
        public int NbOfSuccesses => Successes.Length;
        public int NbOfDiscards => Discards.Length;

        public SmokeTestResultWithMetaDataDto[] Failures => _allResults.Where(x => x.Outcome == false && x.Status != Status.Timeout).ToArray();
        public SmokeTestResultWithMetaDataDto[] Timeouts => _allResults.Where(x => x.Outcome == false && x.Status == Status.Timeout).ToArray();
        public SmokeTestResultWithMetaDataDto[] Successes => _allResults.Where(x => x.Outcome == true && x.Status != Status.Discarded).ToArray();
        public SmokeTestResultWithMetaDataDto[] Discards => _allResults.Where(x => x.Outcome == true && x.Status == Status.Discarded).ToArray();

        public SmokeTestsResults(SmokeTestResultWithMetaDataDto[] allResults)
        {
            _allResults = allResults;
        }
    }
}