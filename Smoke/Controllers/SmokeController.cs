using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Smoke.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SmokeController : ControllerBase
    {
        private readonly IFindSmokeTests _smokeTestProvider;
        private readonly IConfiguration _configuration;

        public SmokeController(IFindSmokeTests smokeTestProvider, IConfiguration configuration)
        {
            _smokeTestProvider = smokeTestProvider;
            _configuration = configuration;
        }

        /// <summary>
        /// Execute all registered Smoke Tests for this API.
        /// </summary>
        /// <returns>The <see cref="SmokeTestSessionResult"/> of the Smoke tests execution.</returns>
        [HttpGet]
        public async Task<SmokeTestSessionResult> RunSmokeTests()
        {
            // Find all smoke tests to run
            var smokeTests = _smokeTestProvider.FindAllSmokeTestsToRun();

            var globalTimeout = TimeSpan.FromSeconds(1);
            if(int.TryParse(_configuration[Constants.GlobaltimeoutinmsecConfigurationKey], out var globalTimeoutInMsec))
            {
                globalTimeout = TimeSpan.FromMilliseconds(globalTimeoutInMsec);
            }

            // Run them in //
            var results = await SmokeTestRunner.ExecuteAllSmokeTests(smokeTests, globalTimeout);

            // Adapt from business to DTO

            return results;
        }
    }
}
