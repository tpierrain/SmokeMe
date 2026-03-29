using Diverse;
using Sample.ExternalSmokeTests.Utilities;
using SmokeMe.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Register SmokeMe services (smoke test discovery + configuration)
builder.Services.AddSmokeMe();

// Register application-specific services needed by smoke tests
builder.Services.AddTransient<IToggleFeatures, AlwaysDisabledToggleFeatureManager>();
builder.Services.AddSingleton<IRestClient, RestClient>();
builder.Services.AddSingleton<IFuzz, Fuzzer>();

var app = builder.Build();

// Map the /smoke endpoint
app.MapSmokeEndpoint();

app.Run();
