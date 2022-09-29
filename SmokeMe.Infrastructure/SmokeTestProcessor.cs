using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SmokeMe.Helpers;
using SmokeMe.Infra;

namespace SmokeMe;

/// <summary>
/// Responsible to run all found smoke tests to retrieve the accurate http status code
/// </summary>
public sealed class SmokeTestProcessor : IProcessSmokeTests
{
    private readonly IConfiguration _configuration;
    private readonly IFindSmokeTests _smokeTestProvider;

    /// <summary>
    /// Instantiates a <see cref="SmokeTestProcessor"/>
    /// </summary>
    /// <param name="configuration">The configuration of the API.</param>
    /// <param name="serviceProvider">A Service provider to be used to instantiate a <see cref="IFindSmokeTests"/> instance if not provided.</param>
    /// <param name="smokeTestProvider">(optional) A smoke test provider (used for unit testing purpose).</param>
    public SmokeTestProcessor(IConfiguration configuration, IServiceProvider serviceProvider, IFindSmokeTests smokeTestProvider = null)
    {
        _configuration = configuration;

        if (serviceProvider == null && smokeTestProvider == null)
        {
            throw new ArgumentNullException("serviceProvider", "Must provide a non-null serviceProvider when smokeTestProvider is not provided.");
        }

        smokeTestProvider ??= new SmokeTestAutoFinder(serviceProvider);
        _smokeTestProvider = smokeTestProvider;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="categories"></param>
    /// <returns></returns>
    public async Task<(int HttpStatusCode, SmokeTestsSessionReportDto SessionReport)> Process(params string[] categories)
    {
        var requestedCategories = categories; // adapt name to our context
        var globalTimeout = _configuration.GetSmokeMeGlobalTimeout();

        if (!_configuration.IsSmokeTestExecutionEnabled())
        {
            return (StatusCodes.Status503ServiceUnavailable,
                new SmokeTestsDisabledReportDto(new ApiRuntimeDescription(), globalTimeout));
        }

        // Find all smoke tests to run
        var smokeTests = _smokeTestProvider.FindAllSmokeTestsToRun(requestedCategories);


        if (ThereIsNoUnignoredSmokeTest(smokeTests))
        {
            int notImplementedHttpStatusCode = StatusCodes.Status501NotImplemented;

            if (requestedCategories.Length > 0)
            {
                return (notImplementedHttpStatusCode,
                    new SmokeTestsSessionReportDto(new ApiRuntimeDescription(), globalTimeout,
                        status: GenerateStatusMessageForNoSmokeTestsWithCategories(requestedCategories)));
            }

            return (notImplementedHttpStatusCode,
                new SmokeTestsSessionReportDto(new ApiRuntimeDescription(), globalTimeout,
                    status:
                    $"No smoke test have been found in your executing assemblies. Start adding (not ignored) {nameof(SmokeTest)} types in your code base so that the SmokeMe library can detect and run them."));
        }

        var results = await SmokeTestRunner.ExecuteAllSmokeTestsInParallel(smokeTests, globalTimeout);

        // Adapt from business to DTO with extra information
        var resultDto = SmokeTestSessionResultAdapter.Adapt(results, new ApiRuntimeDescription(), requestedCategories, _configuration);

        if (resultDto.IsSuccess)
        {
            return (StatusCodes.Status200OK, resultDto);
        }

        if (results is TimeoutSmokeTestsSessionReport)
        {
            return (StatusCodes.Status504GatewayTimeout, resultDto);
        }

        return (StatusCodes.Status500InternalServerError, resultDto);
    }

    private static bool ThereIsNoUnignoredSmokeTest(IEnumerable<SmokeTestInstanceWithMetaData> smokeTests)
    {
        return smokeTests.All(t => t.SmokeTest.GetType().HasIgnoredCustomAttribute());
    }

    private static string GenerateStatusMessageForNoSmokeTestsWithCategories(params string[] categories)
    {
        if (categories.Length == 1)
        {
            return
                @$"No smoke test with [Category(""{categories[0]}"")] attribute have been found in your executing assemblies. Check that you have one or more (not ignored) ICheckSmoke types in your code base with the declared attribute [Category(""{categories[0]}"")] so that the SmokeMe library can detect and run them.";
        }

        var expectedAttributes = categories.Select(s => @$"[Category(""{s}"")]");

        return
            @$"No smoke test with {string.Join(" or ", expectedAttributes)} attributes have been found in your executing assemblies. Check that you have one or more (not ignored) ICheckSmoke types in your code base with the expected declared [Category] attributes so that the SmokeMe library can detect and run them.";
    }
}