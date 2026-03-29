# SmokeMe backlog

1. **Security:** with a nice way for you to plug your own ACL and rights mechanism to the /smoke resource (so that not everyone is able to execute your smoke tests in production)
1. Some tooling so that I can easily reuse/run all my smoke tests in classical acceptance testing sessions
1. A way to easily declare that you want to prevent 2 or more smoke tests to be ran in // if needed (something like a [Mutex("idOfIDoNotWantToHaveMoreOfThoseSmokeTestsToBeRanInParallel")] attribute for some of our Smoke tests)
1. Some *maximum number of smoke tests to be run in parallel* optional and configurable limitation mechanism
1. **Build-time discovery via Source Generator (v4):** Replace runtime reflection (`Assembly.GetTypes()`) with a Roslyn Source Generator that discovers `SmokeTest` subclasses at compile time. Each assembly containing smoke tests would get a generated `SmokeTestRegistry` class. Benefits: Native AOT compatible, faster startup, compile-time validation. Challenge: cross-assembly discovery requires the generator to emit a registry per assembly + a lightweight runtime aggregation step (e.g. scanning for `[assembly: SmokeTestAssembly]` attributes instead of all types)

