# SmokeMe backlog


1. **Security:** with a nice way for you to plug your own ACL and rights mechanism to the /smoke resource (so that not everyone is able to execute your smoke tests in production)
1. Add an [Ignore] attribute to ignore one or more smoke test (useful to keep it in our code base whereas we don't want to run it now
1. Some tooling so that I can easily reuse/run all my smoke tests in classical acceptance testing sessions
1. A way to easily declare that you want to prevent 2 or more smoke tests to be ran in // if needed (something like a [Mutex("idOfIDoNotWantToHaveMoreOfThoseSmokeTestsToBeRanInParallel")] attribute for some of our Smoke tests)
1. Some *maximum number of smoke tests to be run in parallel* optional and configurable limitation mechanism

