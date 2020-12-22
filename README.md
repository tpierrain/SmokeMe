# SmokeMe! (a.k.a.  /smoke ) ![.NET Core](https://github.com/42skillz/Smoke/workflows/.NET%20Core/badge.svg)

A *convention-based* dotnet plugin that will automatically expose all your declared smoke tests behind a  **/smoke** resource in your API.

![twitter screen](https://github.com/42skillz/Smoke/blob/main/Images/smoke.jpg?raw=true)   
  
# 
![twitter icon](https://github.com/42skillz/Smoke/blob/main/Images/Twitter_icon.gif?raw=true) [use case driven on twitter](https://twitter.com/tpierrain) - (thomas@42skillz.com)


## Smoke tests anyone?
Smoke test is preliminary integration testing to reveal simple failures severe enough to, for example, reject a prospective software release. 

The expression came from plumbing where a *smoke test* is a technique forcing non-toxic, artificially created smoke through waste and drain pipes under a slight pressure to find leaks

![twitter screen](https://github.com/42skillz/Smoke/blob/main/Images/swaggered-crop.jpg?raw=true)   

This may differ from classical health checks:

 - **health checks** are sub-second requests made by Load balancers or infrastructure components to your APIs
    - You often just check connectivity with external dependency systems

 - **smoke checks** are (*sub-ten of seconds*) integration tests made by you or your CI scripts just after a deployment
    - You often check *"high-value uses"* of your API to see if it is globally OK
    - This can take more time than a classical health check
    - Note: you can also use them in order to monitor your API overall health


### *"Smoke tests can save your bacon when doing Continuous Delivery!"*

The idea of the **SmokeMe** plugin library is to save you times and let you only focus on writing your functional or technical smoke tests. 

All the auto-discovery, infrastructure and plumbering things are done for you by the pico lib.


## It couldn't be easier!

### A. While coding

1. You add the reference to the **SmokeMe** library in your API project
2. You code all the smoke tests scenario you want in your code base
    - A Smoke test scenario is just a class implementing the **ICheckSmoke** interface with 3 members

```csharp

/// <summary>
/// Smoke test/scenario/code to be executed in order to check that a minimum
/// viable capability of your system is working.
/// 
/// Note: all the services and dependencies you need for it will be automatically
/// injected by the SmokeMe framework via the ASP.NET IServiceProvider of your API
/// (classical constructor-based injection). Can't be that easy, right? ;-)
/// </summary>
public interface ICheckSmoke
{
    /// <summary>
    /// Name of the smoke test scenario.
    /// </summary>
    string SmokeTestName { get; }

    /// <summary>
    /// Description of the smoke test scenario.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// The code of this smoke test scenario.
    /// </summary>
    /// <returns>The <see cref="SmokeTestResult"/> of this Smoke test.</returns>
    Task<SmokeTestResult> Scenario();
}


```

### B. While deploying or supporting your production

You just GET the (automatically added) **/smoke** ressource on your API.

e.g.:

## https://(your-own-api-url):(portnumber)/smoke

or via curl for instance:

```

curl -X GET "https://localhost:5001/smoke" -H  "accept: */*"

```

And you check the HTTP response type you get:

### HTTP 200 (OK)

Means that all your smoke tests have been executed nicely and before the global timeout allowed by **SmokeMe**

![twitter screen](https://github.com/42skillz/Smoke/blob/main/Images/HTTP-200.JPG?raw=true)   


### HTTP 504 (GatewayTimeout)

Means that one or more smoke tests have timeout (configurable global timeout is 10 seconds by default)

![twitter screen](https://github.com/42skillz/Smoke/blob/main/Images/HTTP-504.JPG?raw=true)   


### HTTP 501 (Not implemented)

Means that **SmokeMe** could not find any **ITestSmoke** type within all the assemblies 
that have been loaded into the execution context of this API.

![twitter screen](https://github.com/42skillz/Smoke/blob/main/Images/HTTP-501.JPG?raw=true)   


### HTTP 500 (Internal Server Error)

Means that **SmokeMe** has executed all your declared **ITestSmoke** type instances but there have been 
at least one failing smoke test.

![twitter screen](https://github.com/42skillz/Smoke/blob/main/Images/HTTP-500.JPG?raw=true)   


### HTTP 503 (Service Unavailable)

Means that smoke test execution has been disabled via configuration.

![twitter screen](https://github.com/42skillz/Smoke/blob/main/Images/HTTP-503.JPG?raw=true)   


---

## FAQ

### 1. Does SmokeMe execute all your founded smoke tests in parallel?

```
Yes
```

### 2. Does SmokeMe have a global timeout for all smoke tests to be ran?

```
Yes. It's 10 seconds by default (10 *1000 milliseconds). But you can override this 
default value by setting the **Smoke:GlobalTimeoutInMsec** configuration key 
of your Web API project.

```

### 3. How to make SmokeMe being able to execute all my smoke tests?

```
More than easy. All you have to do is to add a reference to the **SmokeMe** lib 
in your API project for it to be able to find all of them. That's it!
```

### 4. How to code and declare a smoke test?

```
Easy, all you have to do is to add a reference to the **SmokeMe** lib in your 
code and to code a smoke test by implementing a type implementing 
SmokeMe.ICheckSmoke interface.

```

e.g.: 

```csharp

/// <summary>
/// Smoke test to check that room availabilities works and is accessible.
/// </summary>
public class AvailabilitiesSmokeTest : ICheckSmoke
{
    private readonly IAvailabilityService _availabilityService;
    public string SmokeTestName => "Check Availabilities";
    public string Description 
        => "TBD: will check something like checking that one can find some availabilities around Marseille city next month.";

    /// <summary>
    /// Instantiates a <see cref="AvailabilitiesSmokeTest"/>
    /// </summary>
    /// <param name="availabilityService">The <see cref="IAvailabilityService"/> we need (will be 
    /// automatically injected par the SmokeMe library)</param>
    public AvailabilitiesSmokeTest(IAvailabilityService availabilityService)
    {
        // availability service here is just an example of 
        // on of your own API-level registered service automatically
        // injected to your smoke test instance by the SmokeMe lib
        _availabilityService = availabilityService;
    }
        
    /// <summary>
    /// The implementation of this smoke test scenario.
    /// </summary>
    /// <returns>The result of the Smoke test.</returns>
    public Task<SmokeTestResult> Scenario()
    {
        if (_availabilityService != null)
        {
            // TODO: implement our smoke test here
            // (the one using the _availabilityService to check hotels' rooms availability)
            return Task.FromResult(new SmokeTestResult(true));
        }

        return Task.FromResult(new SmokeTestResult(false));
    }
}

```

### 5. How can I avoid the issue of having error: '"code": "ApiVersionUnspecified" ' when calling /smoke?

```
This issue is due to the fact that your API requires an explicit version for every Controller 
whereas the SmokeMe.SmokeController does not have one on purpose (to avoid crashing 
when one does not have an explicit versioning configuration nor references 
to Microsoft.AspNetCore.Mvc.Versioning & Co in its API).

As a consequence, the /smoke route for your smoke test won't be coupled to any version 
like /api/v1/ etc. but will be available instead from the root of your API /smoke.

Fortunately the error that may occurs when calling /smoke in those cases may be fixed by a simple option within your API Startup type:

options.AssumeDefaultVersionWhenUnspecified = true;

at the services.AddApiVersioning(...) method invocation level.


```
e.g.: 

```csharp

services.AddApiVersioning(
    options =>
    {
        options.ReportApiVersions = true;
        options.DefaultApiVersion = new ApiVersion(0,0);
        options.AssumeDefaultVersionWhenUnspecified = true; // the line you should add in case of problem
    } );

```


### 6. How can I disable the execution of smoke test?

```
Just set false to the "Smoke:IsSmokeTestExecutionEnabled" configuration key (default value is true).

e.g.:

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Smoke": {
    "GlobalTimeoutInMsec": 1500,
    "IsSmokeTestExecutionEnabled": false
  },
  "AllowedHosts": "*"
}


```

---

## Next steps

 - **Security:** with a nice way for you to plug your own ACL and rights mechanism to the /smoke resource (so that not everyone is able to execute your smoke tests in production)
 - Some tooling so that I can easily reuse/run all my smoke tests in classical acceptance testing sessions
 - A way to easily declare that you want to prevent 2 or more smoke tests to be ran in // if needed (something like a [Mutex("idOfIDoNotWantToHaveMoreOfThoseSmokeTestsToBeRanInParallel")] attribute for some of our Smoke tests)
 - Some *maximum number of smoke tests to be run in parallel* optional and configurable limitation mechanism

 More on this [here](./Backlog.md)


---

## Hope you will enjoy it!

We value your input and appreciate your feedback. Thus, don't hesitate to leave them on the [**github issues of the project**](https://github.com/42skillz/SmokeMe/issues).




