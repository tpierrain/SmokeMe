using System.Linq;
using SmokeMe.Helpers;

namespace SmokeMe.Infra
{
    /// <summary>
    /// Adapter from SmokeMe internal model to SmokeMe external DTOs.
    /// </summary>
    public static class SmokeTestSessionResultAdapter
    {
        /// <summary>
        /// Adapts a <see cref="SmokeTestsSessionReport"/> instance to a <see cref="SmokeTestsSessionReportDto"/> one.
        /// </summary>
        /// <param name="reports">The <see cref="SmokeTestsSessionReport"/> instance to adapt.</param>
        /// <param name="runtimeDescription">The <see cref="ApiRuntimeDescription"/> to associate</param>
        /// <param name="categories"></param>
        /// <returns>The <see cref="SmokeTestsSessionReportDto"/> corresponding to the external exposition model of the provided <see cref="SmokeTestsSessionReport"/> instance.</returns>
        public static SmokeTestsSessionReportDto Adapt(SmokeTestsSessionReport reports, ApiRuntimeDescription runtimeDescription, string[] categories)
        {
            // Adapt the array of results 
            var resultsDto = reports.Results
                .Select(r => new SmokeTestResultWithMetaDataDto(r.SmokeTestName, r.SmokeTestDescription, r.Outcome, r.ErrorMessage, r.Duration, r.Duration?.GetHumanReadableVersion(), r.Status, r.SmokeTestCategories, r.SmokeTestType));

            // Adapt the overall wrapper (with runtime description information too)
            var result = new SmokeTestsSessionReportDto(reports, runtimeDescription, resultsDto, categories);

            return result;
        }
    }
}