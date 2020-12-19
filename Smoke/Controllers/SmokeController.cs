using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Smoke.Controllers
{
    /// <summary>
    /// Executes smoke test declared for this API.
    /// Smoke tests are a set of short functional tests checking that the minimum viable prerequisites for this API is fine.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SmokeController : ControllerBase
    {
        private readonly IFindSmokeTests _smokeTestProvider;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Instantiates a <see cref="SmokeController"/>.
        /// </summary>
        /// <param name="configuration">The configuration of the API.</param>
        /// <param name="serviceProvider">A Service provider to be used to instantiate <see cref="ISmokeTestAScenario"/> smoke tests.</param>
        /// <param name="smokeTestProvider">(optional) A smoke test provider (used for unit testing purpose).</param>
        public SmokeController(IConfiguration configuration, IServiceProvider serviceProvider, IFindSmokeTests smokeTestProvider = null)
        {
            _configuration = configuration;

            if (serviceProvider == null && smokeTestProvider == null)
            {
                throw new ArgumentNullException("serviceProvider", "Must provide a non-null serviceProvider when smokeTestProvider is not provided.");
            }

            smokeTestProvider??= new SmokeTestAutoFinder(serviceProvider);
            _smokeTestProvider = smokeTestProvider;
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

            // Execute them in //
            var results = await SmokeTestRunner.ExecuteAllSmokeTests(smokeTests, globalTimeout);

            // Adapt from business to DTO

            return results;
        }
    }
}
