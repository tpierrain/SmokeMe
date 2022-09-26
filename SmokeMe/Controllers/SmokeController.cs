using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace SmokeMe.Controllers
{
    /// <summary>
    /// Executes smoke test declared for this API.
    /// Smoke tests are a set of short functional tests checking that the minimum viable prerequisites for this API is fine.
    /// </summary>
    [ApiController]
    [Route("smoke")]
    public class SmokeController : ControllerBase
    {
        private readonly IProcessSmokeTests _smokeTestProcessor;

        /// <summary>
        /// Instantiates a <see cref="SmokeController"/>.
        /// </summary>
        /// <param name="configuration">The configuration of the API.</param>
        /// <param name="serviceProvider">A Service provider to be used to instantiate <see cref="SmokeTest"/> smoke tests.</param>
        /// <param name="smokeTestProcessor">(optional) A smoke test provider (used for unit testing purpose).</param>
        public SmokeController(IConfiguration configuration, IServiceProvider serviceProvider, IProcessSmokeTests smokeTestProcessor = null)
        {
            if (serviceProvider == null && smokeTestProcessor == null)
            {
                throw new ArgumentNullException("serviceProvider", "Must provide a non-null serviceProvider when smokeTestProvider is not provided.");
            }

            smokeTestProcessor??= new SmokeTestProcessor(configuration, serviceProvider);
            _smokeTestProcessor = smokeTestProcessor;
        }

        /// <summary>
        /// Execute all registered Smoke Tests for this API.
        /// </summary>
        /// <returns>The <see cref="SmokeTestsSessionReport"/> of the Smoke tests execution.</returns>
        [HttpGet]
        public async Task<IActionResult> ExecuteSmokeTests([FromQuery] params string[] categories)
        {
            var (httpStatusCode, sessionReport) = await _smokeTestProcessor.Process(categories);

            return new ObjectResult(sessionReport)
            {
                StatusCode = httpStatusCode
            };
        }
    }
}
