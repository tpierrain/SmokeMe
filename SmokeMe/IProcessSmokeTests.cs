using System.Threading.Tasks;
using SmokeMe.Infra;

namespace SmokeMe;

/// <summary>
/// Responsible to run all found smoke tests.
/// </summary>
public interface IProcessSmokeTests
{
    /// <summary>
    /// Run all the instantiated <see cref="SmokeTest"/> instances that have been found in the running code.
    /// </summary>
    /// <returns>A collection of <see cref="SmokeTest"/> instances.</returns>
    Task<(int HttpStatusCode, SmokeTestsSessionReportDto SessionReport)> Process(params string[] categories);
}