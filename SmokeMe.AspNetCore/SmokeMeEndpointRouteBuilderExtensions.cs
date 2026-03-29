using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SmokeMe.Helpers;
using SmokeMe.Infra;

namespace SmokeMe.AspNetCore
{
    /// <summary>
    /// Extension methods for <see cref="IEndpointRouteBuilder"/> to map the SmokeMe endpoint.
    /// </summary>
    public static class SmokeMeEndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Maps a GET endpoint that executes all registered smoke tests.
        /// </summary>
        /// <param name="endpoints">The endpoint route builder.</param>
        /// <param name="pattern">The URL pattern for the smoke endpoint. Default: "/smoke".</param>
        /// <returns>The <see cref="IEndpointConventionBuilder"/> for further configuration.</returns>
        public static IEndpointConventionBuilder MapSmokeEndpoint(this IEndpointRouteBuilder endpoints, string pattern = "/smoke")
        {
            return endpoints.MapGet(pattern, async (HttpContext context) =>
            {
                var configuration = context.RequestServices.GetRequiredService<ISmokeTestConfiguration>();
                var smokeTestProvider = context.RequestServices.GetRequiredService<IFindSmokeTests>();

                var globalTimeout = configuration.GlobalTimeout;

                if (!configuration.IsExecutionEnabled)
                {
                    return Results.Json(new SmokeTestsDisabledReportDto(new ApiRuntimeDescription(), globalTimeout),
                        statusCode: (int)HttpStatusCode.ServiceUnavailable);
                }

                var requestedCategories = context.Request.Query.ContainsKey("categories")
                    ? context.Request.Query["categories"].ToArray()
                    : new string[0];

                var smokeTests = smokeTestProvider.FindAllSmokeTestsToRun(requestedCategories);

                if (ThereIsNoUnignoredSmokeTest(smokeTests))
                {
                    if (requestedCategories.Length > 0)
                    {
                        return Results.Json(
                            new SmokeTestsSessionReportDto(new ApiRuntimeDescription(), globalTimeout,
                                status: GenerateStatusMessageForNoSmokeTestsWithCategories(requestedCategories)),
                            statusCode: (int)HttpStatusCode.NotImplemented);
                    }

                    return Results.Json(
                        new SmokeTestsSessionReportDto(new ApiRuntimeDescription(), globalTimeout,
                            status: $"No smoke test have been found in your executing assemblies. Start adding (not ignored) {nameof(SmokeTest)} types in your code base so that the SmokeMe library can detect and run them."),
                        statusCode: (int)HttpStatusCode.NotImplemented);
                }

                var results = await SmokeTestRunner.ExecuteAllSmokeTestsInParallel(smokeTests, globalTimeout);

                var resultDto = SmokeTestSessionResultAdapter.Adapt(results, new ApiRuntimeDescription(), requestedCategories, configuration);

                if (resultDto.IsSuccess)
                {
                    return Results.Json(resultDto, statusCode: (int)HttpStatusCode.OK);
                }

                if (results is TimeoutSmokeTestsSessionReport)
                {
                    return Results.Json(resultDto, statusCode: (int)HttpStatusCode.GatewayTimeout);
                }

                return Results.Json(resultDto, statusCode: (int)HttpStatusCode.InternalServerError);
            });
        }

        private static bool ThereIsNoUnignoredSmokeTest(IEnumerable<SmokeTestInstanceWithMetaData> smokeTests)
        {
            return !smokeTests.Any(t => !t.SmokeTest.GetType().HasIgnoredCustomAttribute());
        }

        private static string GenerateStatusMessageForNoSmokeTestsWithCategories(params string[] categories)
        {
            if (categories.Length == 1)
            {
                return @$"No smoke test with [Category(""{categories[0]}"")] attribute have been found in your executing assemblies. Check that you have one or more (not ignored) {nameof(SmokeTest)} types in your code base with the declared attribute [Category(""{categories[0]}"")] so that the SmokeMe library can detect and run them.";
            }

            var expectedAttributes = categories.Select(s => @$"[Category(""{s}"")]");

            return @$"No smoke test with {string.Join(" or ", expectedAttributes)} attributes have been found in your executing assemblies. Check that you have one or more (not ignored) {nameof(SmokeTest)} types in your code base with the expected declared [Category] attributes so that the SmokeMe library can detect and run them.";
        }
    }
}
