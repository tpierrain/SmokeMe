using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using SmokeMe;

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
            .MapGet(endpoint, async (HttpContext context, [FromQuery] string? category) =>
            {
                var processor = InstantiateSmokeTestProcessor(context.RequestServices);
                var (httpStatusCode, sessionReport) = await processor.Process(category);

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