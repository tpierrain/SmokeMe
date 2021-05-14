using System.Net;
using System.Threading.Tasks;
using Sample.ExternalSmokeTests.Utilities;
using SmokeMe;

namespace Sample.ExternalSmokeTests
{
    /// <summary>
    /// Smoke test only to check that SmokeMe is able to detect and run all <see cref="SmokeTest"/>
    /// types wherever they are located (i.e. in other assemblies than the API).
    /// </summary>
    [Category("Connectivity")]
    public class SmokeTestGoogleConnectivityLocatedInAnotherAssembly : SmokeTest
    {
        private readonly IRestClient _restClient;

        public override string SmokeTestName => "Check connectivity towards Google search engine.";
        public override string Description => "Check that the Google search engine is reachable";

        public SmokeTestGoogleConnectivityLocatedInAnotherAssembly(IRestClient restClient)
        {
            _restClient = restClient;
        }

        public override async Task<SmokeTestResult> Scenario()
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