using System.Net;
using System.Threading.Tasks;
using Sample.ExternalSmokeTests.Utilities;
using SmokeMe;

namespace Sample.ExternalSmokeTests
{
    /// <summary>
    /// Smoke test only to check that SmokeMe is able to detect and run all <see cref="ISmokeTestAScenario"/>
    /// types wherever they are located (i.e. in other assemblies than the API).
    /// </summary>
    public class SmokeTestLocatedInAnotherAssembly : ISmokeTestAScenario
    {
        private readonly IRestClient _restClient;

        public string SmokeTestName => "Check connectivity towards Google search engine.";
        public string Description => "Check that the Google search engine is reachable";

        public SmokeTestLocatedInAnotherAssembly(IRestClient restClient)
        {
            _restClient = restClient;
        }

        public async Task<SmokeTestResult> ExecuteScenario()
        {
            // check if Google is still here ;-)

            const string url = "https://www.google.com/";

            var response = await _restClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return new SmokeTestResult(true);
            }

            return new SmokeTestResult(false);
        }
    }
}