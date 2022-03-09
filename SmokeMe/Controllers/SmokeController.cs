using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SmokeMe.Helpers;
using SmokeMe.Infra;

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
        private readonly IFindSmokeTests _smokeTestProvider;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Instantiates a <see cref="SmokeController"/>.
        /// </summary>
        /// <param name="configuration">The configuration of the API.</param>
        /// <param name="serviceProvider">A Service provider to be used to instantiate <see cref="SmokeTest"/> smoke tests.</param>
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
        /// <returns>The <see cref="SmokeTestsSessionReport"/> of the Smoke tests execution.</returns>
        [HttpGet]
        public async Task<IActionResult> ExecuteSmokeTests([FromQuery] params string[] categories)
        {
            var requestedCategories = categories; // adapt name to our context
            var globalTimeout = _configuration.GetSmokeMeGlobalTimeout();

            if (!_configuration.IsSmokeTestExecutionEnabled())
            {
                return StatusCode((int) HttpStatusCode.ServiceUnavailable, new SmokeTestsDisabledReportDto(new ApiRuntimeDescription(), globalTimeout));
            }

            // Find all smoke tests to run
            var smokeTests = _smokeTestProvider.FindAllSmokeTestsToRun(requestedCategories);


            if (ThereIsNoUnignoredSmokeTest(smokeTests))
            {
                if (requestedCategories.Length > 0)
                {
                    return StatusCode((int)HttpStatusCode.NotImplemented, new SmokeTestsSessionReportDto(new ApiRuntimeDescription(), globalTimeout, status: GenerateStatusMessageForNoSmokeTestsWithCategories(requestedCategories)));
                }
                
                return StatusCode((int) HttpStatusCode.NotImplemented, new SmokeTestsSessionReportDto(new ApiRuntimeDescription(), globalTimeout, status: $"No smoke test have been found in your executing assemblies. Start adding (not ignored) {nameof(SmokeTest)} types in your code base so that the SmokeMe library can detect and run them."));
            }

            var results = await SmokeTestRunner.ExecuteAllSmokeTestsInParallel(smokeTests, globalTimeout);

            // Adapt from business to DTO with extra information
            var resultDto = SmokeTestSessionResultAdapter.Adapt(results, new ApiRuntimeDescription(), requestedCategories, _configuration);

            if (resultDto.IsSuccess)
            {
                return Ok(resultDto);
            }

            if (results is TimeoutSmokeTestsSessionReport)
            {
                return StatusCode((int) HttpStatusCode.GatewayTimeout, resultDto);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, resultDto);
        }

        private static bool ThereIsNoUnignoredSmokeTest(IEnumerable<SmokeTestInstanceWithMetaData> smokeTests)
        {
            return !smokeTests.Any(t=> !t.SmokeTest.GetType().HasIgnoredCustomAttribute());
        }

        private static string GenerateStatusMessageForNoSmokeTestsWithCategories(params string[] categories)
        {
            if (categories.Length == 1)
            {
                return @$"No smoke test with [Category(""{categories[0]}"")] attribute have been found in your executing assemblies. Check that you have one or more (not ignored) ICheckSmoke types in your code base with the declared attribute [Category(""{categories[0]}"")] so that the SmokeMe library can detect and run them.";
            }

            var expectedAttributes = categories.Select(s => @$"[Category(""{s}"")]");
            
            return @$"No smoke test with {string.Join(" or ", expectedAttributes)} attributes have been found in your executing assemblies. Check that you have one or more (not ignored) ICheckSmoke types in your code base with the expected declared [Category] attributes so that the SmokeMe library can detect and run them.";
        }
    }
}
