# Migration Guide: SmokeMe v2 to v3

## Why v3?

SmokeMe v2 was a single NuGet package targeting `netcoreapp3.1` (EOL since December 2022). It embedded an MVC controller, Swagger/OpenAPI dependencies, Newtonsoft.Json, and API versioning — all baked in, whether you needed them or not.

v3 fixes this by:

1. **Splitting into two packages** — a framework-agnostic core and an ASP.NET Core integration layer
2. **Targeting supported runtimes** — `netstandard2.0` (core) + `net8.0`/`net9.0` (ASP.NET integration)
3. **Removing heavy dependencies** — no more Newtonsoft.Json, Swashbuckle, or API versioning pulled in automatically
4. **Making registration explicit** — you opt-in with `AddSmokeMe()` + `MapSmokeEndpoint()` instead of relying on convention-based controller discovery

## What changed

| Area | v2 | v3 |
|------|----|----|
| **NuGet packages** | 1 package: `SmokeMe` | 2 packages: `SmokeMe` (core) + `SmokeMe.AspNetCore` (integration) |
| **Target framework** | `netcoreapp3.1` | `netstandard2.0` (core) + `net8.0`/`net9.0` (ASP.NET) |
| **Registration** | Automatic (MVC controller discovered by convention) | Explicit: `AddSmokeMe()` + `MapSmokeEndpoint()` |
| **Endpoint style** | MVC Controller (`SmokeController`) | Minimal API endpoint (via `MapSmokeEndpoint()`) |
| **JSON serializer** | Newtonsoft.Json | System.Text.Json |
| **Embedded dependencies** | Swashbuckle, API Versioning, Newtonsoft.Json | None — only `System.Text.Json` (core) and `Microsoft.AspNetCore.App` framework reference |
| **Configuration** | `IConfiguration` extension methods (`GetSmokeMeGlobalTimeout()`, `IsSmokeTestExecutionEnabled()`) | `ISmokeTestConfiguration` interface (auto-bridged from `appsettings.json`) |
| **`ICheckSmoke` interface** | Present (deprecated since v2) | Removed |

## Step-by-step migration

### 1. Update NuGet references

Remove the old `SmokeMe` package and install both v3 packages:

```xml
<!-- Remove -->
<PackageReference Include="SmokeMe" Version="2.x.x" />

<!-- Add -->
<PackageReference Include="SmokeMe" Version="3.0.0" />
<PackageReference Include="SmokeMe.AspNetCore" Version="3.0.0" />
```

> If you have smoke tests in a **separate assembly** (class library), that assembly only needs the `SmokeMe` package (core). Only your **web host** project needs `SmokeMe.AspNetCore`.

### 2. Register services and map the endpoint

**v2** — no registration needed (the `SmokeController` was discovered automatically by MVC):

```csharp
// Startup.cs — v2: nothing special, the controller just worked
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    // ...
}
```

**v3** — explicit registration required:

```csharp
// Program.cs (Minimal API style)
using SmokeMe.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSmokeMe(); // registers smoke test discovery + configuration

var app = builder.Build();

app.MapSmokeEndpoint(); // GET /smoke (default path)

app.Run();
```

Or if you use a custom path:

```csharp
app.MapSmokeEndpoint("/healthcheck/smoke");
```

### 3. Remove `ICheckSmoke` usages (if any remain)

`ICheckSmoke` was deprecated in v2 and has been **removed** in v3. If you still have types implementing it:

```csharp
// v2 (deprecated)
public class MyTest : ICheckSmoke { ... }

// v3
public class MyTest : SmokeTest
{
    public override string SmokeTestName => "My test";
    public override string Description => "Checks something";
    public override async Task<SmokeTestResult> Scenario() { ... }
}
```

### 4. Update configuration access (if you used extension methods)

v2 provided extension methods on `IConfiguration`:

```csharp
// v2
var timeout = configuration.GetSmokeMeGlobalTimeout();
var isEnabled = configuration.IsSmokeTestExecutionEnabled();
```

These extension methods have been **removed**. In v3, configuration is read automatically from `appsettings.json` by the `AddSmokeMe()` registration. The configuration keys are unchanged:

```json
{
  "Smoke": {
    "GlobalTimeoutInMsec": 30000,
    "IsSmokeTestExecutionEnabled": true
  }
}
```

If you need programmatic configuration:

```csharp
builder.Services.AddSmokeMe(options =>
{
    options.GlobalTimeout = TimeSpan.FromSeconds(60);
    options.IsExecutionEnabled = true;
});
```

`appsettings.json` values take precedence over programmatic defaults when both are present.

### 5. Handle JSON serialization differences

v3 uses **System.Text.Json** instead of Newtonsoft.Json. This matters if you were relying on specific Newtonsoft behaviors in your smoke test responses:

- Property names are now **camelCase by default** (System.Text.Json default) instead of the Newtonsoft default
- If you were parsing the `/smoke` response in external tools, verify the JSON shape still matches your expectations

### 6. Remove Swagger/API versioning workarounds (if any)

v2 pulled in Swashbuckle and `Microsoft.AspNetCore.Mvc.Versioning`. If you had added configurations or workarounds because of these transitive dependencies, you can safely remove them. v3 has no opinion on Swagger or API versioning.

## What about the legacy `SmokeController`?

The `SmokeMe.AspNetCore` package still ships a `SmokeController` marked `[Obsolete]`. This is a **temporary migration aid**:

- If you call `AddSmokeMe()` + `MapSmokeEndpoint()`, the Minimal API endpoint is used (recommended)
- If you still use `AddControllers()` and MVC routing, the legacy `SmokeController` will also be discovered — both can coexist
- The `SmokeController` will be **removed in v4**

**Recommendation:** migrate to `MapSmokeEndpoint()` now. If you see an `[Obsolete]` compiler warning about `SmokeController`, it means something is still referencing it.

## Smoke test classes: no changes needed

The `SmokeTest` abstract base class is **unchanged**. Your existing smoke test implementations (`SmokeTestName`, `Description`, `Scenario()`, `HasToBeDiscarded()`) work as-is. Constructor injection via `IServiceProvider` also works identically.

## Compatibility matrix

| .NET version | SmokeMe (core) | SmokeMe.AspNetCore |
|---|---|---|
| .NET 8 (LTS) | Yes (via netstandard2.0) | Yes |
| .NET 9 | Yes (via netstandard2.0) | Yes |
| .NET Framework 4.6.1+ | Yes (via netstandard2.0) | No (ASP.NET Core only) |
| .NET Core 3.1 / .NET 5-7 | Yes (via netstandard2.0) | No (EOL runtimes) |

## TL;DR — minimal migration checklist

- [ ] Replace `SmokeMe` v2 package with `SmokeMe` + `SmokeMe.AspNetCore` v3
- [ ] Add `builder.Services.AddSmokeMe()` in your service registration
- [ ] Add `app.MapSmokeEndpoint()` in your endpoint configuration
- [ ] Replace any `ICheckSmoke` implementations with `SmokeTest` (if not already done)
- [ ] Remove any calls to `configuration.GetSmokeMeGlobalTimeout()` or `configuration.IsSmokeTestExecutionEnabled()`
- [ ] Verify your `appsettings.json` `Smoke:` section still works (keys unchanged)
- [ ] Run your smoke tests to confirm the `/smoke` endpoint responds correctly
