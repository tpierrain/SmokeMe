# Smoke ![.NET Core](https://github.com/42skillz/Smoke/workflows/.NET%20Core/badge.svg)

A convention-based mini framework allowing you to execute all your declared smoke tests whenever one call the **/smoke** ressource on your own API.

![twitter screen](https://github.com/42skillz/Smoke/blob/main/Images/smoke.jpg?raw=true)   
  
# 
![twitter icon](https://github.com/42skillz/Smoke/blob/main/Images/Twitter_icon.gif?raw=true) [use case driven on twitter](https://twitter.com/tpierrain) - (thomas@42skillz.com)


## Can't be more easy!

1. You add the reference to the **Smoke** library in your API project
2. You code all the smoke tests scenario you want in your code base
    - A Smoke test scenario is just a class implementing the **ISmokeTestAScenario** interface

```csharp

/// <summary>
/// Contains scenario to be executed in order to 'smoke test' something.
/// (a Smoke test actually).
/// Note: all the services and dependencies you need for it will be automatically
/// injected by the lib via the ASP.NET IServiceProvider of your API
/// (classical constructor-based injection).
/// </summary>
public interface ISmokeTestAScenario
{
    /// <summary>
    /// Executes the scenario of this Smoke Test.
    /// </summary>
    /// <returns>The <see cref="SmokeTestResult"/> of this Smoke test.</returns>
    SmokeTestResult ExecuteScenario();
}

```

3. You start your API and just call the /smoke ressource on it. 
    - If all your smoke tests were successfully executed, you get an HTTP 200 code ;-)





