# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**SmokeMe** is a convention-based .NET library (v3.1.0) split into two NuGet packages:
- **SmokeMe** (core, netstandard2.0) — framework-agnostic: reflection discovery, execution, reporting. Zero ASP.NET dependency.
- **SmokeMe.AspNetCore** (net8.0/net9.0/net10.0) — ASP.NET Core integration via `AddSmokeMe()` + `MapSmokeEndpoint()`.

Author: Thomas PIERRAIN (42 skillz).

## Build & Test Commands

```bash
# Restore + build
dotnet build SmokeMe.sln

# Run all tests
dotnet test SmokeMe.sln

# Run a single test by name
dotnet test SmokeMe.Tests/SmokeMe.Tests.csproj --filter "FullyQualifiedName~SmokeControllerShould.Return_OK_200"

# Pack both NuGet packages
dotnet pack SmokeMe/SmokeMe.csproj --configuration Release
dotnet pack SmokeMe.AspNetCore/SmokeMe.AspNetCore.csproj --configuration Release
```

## Architecture

### Core Flow

```
SmokeTestAutoFinder (reflection discovery + DI instantiation)
       │
       ▼
MapSmokeEndpoint (GET /smoke?categories=X) ──► SmokeTestRunner (parallel TPL execution with global timeout)
       │                                              │
       ▼                                              ▼
SmokeTestSessionResultAdapter ◄──── SmokeTestsSessionReport / TimeoutSmokeTestsSessionReport
       │
       ▼
SmokeTestsSessionReportDto (HTTP response)
```

### Key Types

**SmokeMe (core):**
- **`SmokeTest`** — Abstract base class. Consumers implement `SmokeTestName`, `Description`, `Scenario()`. Optional: `HasToBeDiscarded()` for feature-toggle integration, `CleanUp()` for post-test cleanup.
- **`ISmokeTestConfiguration`** — Abstraction for timeout + enabled flag. Decoupled from ASP.NET's `IConfiguration`.
- **`SmokeMeOptions`** — Default implementation of `ISmokeTestConfiguration`.
- **`SmokeTestAutoFinder`** (`IFindSmokeTests`) — Scans all loaded assemblies for `SmokeTest` subclasses, filters by `[Category]`/`[Ignore]`, instantiates via `IServiceProvider`.
- **`SmokeTestRunner`** — Static `ExecuteAllSmokeTestsInParallel()`. Uses `Task.WhenAny` for global timeout enforcement.

**SmokeMe.AspNetCore:**
- **`SmokeMeServiceCollectionExtensions.AddSmokeMe()`** — Registers `ISmokeTestConfiguration` + `IFindSmokeTests` in DI.
- **`SmokeMeEndpointRouteBuilderExtensions.MapSmokeEndpoint()`** — Minimal API endpoint. HTTP status codes: 200 (all passed), 500 (failures), 504 (timeout), 501 (no tests found), 503 (disabled via config).
- **`SmokeMeConfigurationAdapter`** — Bridges ASP.NET `IConfiguration` to `ISmokeTestConfiguration`.
- **`SmokeController`** — Legacy MVC controller (marked `[Obsolete]`, will be removed in v4).

### Configuration Keys (appsettings.json)

```json
{
  "Smoke": {
    "GlobalTimeoutInMsec": 30000,
    "IsSmokeTestExecutionEnabled": true
  }
}
```

Defaults defined in `Constants.cs`.

## Solution Structure

| Project | Target | Purpose |
|---------|--------|---------|
| `SmokeMe/` | netstandard2.0 | Core library (NuGet package) |
| `SmokeMe.AspNetCore/` | net8.0;net9.0;net10.0 | ASP.NET Core integration (NuGet package) |
| `SmokeMe.Tests/` | net10.0 | Tests (NUnit + NFluent + NSubstitute + Diverse) |
| `Samples/Sample.Api/` | net10.0 | Example API using `AddSmokeMe()` + `MapSmokeEndpoint()` |
| `Samples/Sample.ExternalSmokeTests/` | netstandard2.0 | Smoke tests in a separate assembly |

## Design Philosophy

- **Simplicity first**: always favor the simplest and most readable solution. Avoid over-engineering, unnecessary abstractions, or clever workarounds when a straightforward approach exists.
- When adding test doubles (smoke tests for testing purposes), remember that `SmokeTestAutoFinder` discovers ALL `SmokeTest` subclasses via reflection across loaded assemblies — simply update existing test counters to account for new types rather than adding complex mechanisms to hide them.

## Testing Conventions

- **NUnit** framework, **NFluent** assertions (`Check.That(...)`), **NSubstitute** for mocking, **Diverse** (`Fuzzer`) for test data generation.
- Acceptance tests dominate — they exercise `SmokeController` end-to-end with stubbed dependencies.
- Test fixtures use a **`Stub`** helper class (in `SmokeMe.Tests/Helpers/Stub.cs`) to build `ISmokeTestConfiguration`, `IConfiguration` and `IFindSmokeTests` instances.
- Tests are fast (no I/O), isolated (no shared `[SetUp]` state).

## CI

GitHub Actions (`.github/workflows/dotnet.yml`): builds on Ubuntu with .NET 8 + 9, runs tests, creates both NuGet packages on push to main.
