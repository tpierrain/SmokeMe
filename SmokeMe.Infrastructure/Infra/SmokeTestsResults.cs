namespace SmokeMe.Infra
{
    public class SmokeTestsResults
    {
        private readonly SmokeTestResultWithMetaDataDto[] _allResults;

        public int TotalOfTestsDetected => _allResults.Length;
        public int TotalOfTestsRan => Successes.Length + Failures.Length + Timeouts.Length;
        public int NbOfTimeouts => Timeouts.Length;
        public int NbOfFailures => Failures.Length;
        public int NbOfSuccesses => Successes.Length;
        public int NbOfDiscards => Discards.Length;
        public int NbOfIgnoredTests => IgnoredTests.Length;

        public SmokeTestResultWithMetaDataDto[] Timeouts => _allResults.Where(x => x.Outcome == false && x.Status == Status.Timeout).ToArray();
        public SmokeTestResultWithMetaDataDto[] Failures => _allResults.Where(x => x.Outcome == false && x.Status != Status.Timeout).ToArray();
        public SmokeTestResultWithMetaDataDto[] Successes => _allResults.Where(x => x.Outcome == true && x.Status != Status.Discarded && x.Status != Status.Ignored).ToArray();
        public SmokeTestResultWithMetaDataDto[] Discards => _allResults.Where(x => x.Outcome == true && x.Status == Status.Discarded).ToArray();
        public SmokeTestResultWithMetaDataDto[] IgnoredTests => _allResults.Where(x => x.Status == Status.Ignored).ToArray();

        public SmokeTestsResults(SmokeTestResultWithMetaDataDto[] allResults)
        {
            _allResults = allResults;
        }
    }
}