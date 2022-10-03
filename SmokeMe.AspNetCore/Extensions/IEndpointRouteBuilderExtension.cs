using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using SmokeMe;
using SmokeMe.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class IEndpointRouteBuilderExtension
{
    public static RouteHandlerBuilder MapSmokeTests(this IEndpointRouteBuilder builder)
    {
        return builder.MapSmokeTests("smoke");
    }
    
    public static RouteHandlerBuilder MapSmokeTests(this IEndpointRouteBuilder builder, string endpoint)
    {
        return builder
            .MapGet(endpoint, async (HttpContext context, [FromQuery] StringList? categories) =>
            {
                var processor = InstantiateSmokeTestProcessor(context.RequestServices);
                var (httpStatusCode, sessionReport) = await processor.Process(categories);

                return Results.Json(sessionReport, statusCode: httpStatusCode);
            });
    }

    private static IProcessSmokeTests InstantiateSmokeTestProcessor(IServiceProvider provider)
    {
        IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
        IProcessSmokeTests? processor = provider.GetService<IProcessSmokeTests>();

        return processor ?? new SmokeTestProcessor(configuration, provider);
    }
}