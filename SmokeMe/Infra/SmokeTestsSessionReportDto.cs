using System.Collections.Generic;
using System.Linq;

namespace SmokeMe.Infra
{
    /// <summary>
    /// Exposition model of a smoke test session execution.
    /// Contains extra data about the technical execution context too (e.g.: the instance identifier of the API, its number of cores).
    /// </summary>
    public class SmokeTestsSessionReportDto
    {
        private readonly SmokeTestsSessionReport _reports;
        private readonly ApiRuntimeDescription _apiRuntimeDescription;

        /// <summary>
        /// Returns <b>true</b> if the Smoke test session is succeeded (i.e. all smoke test succeeded), <b>false</b> otherwise.
        /// </summary>
        public bool IsSuccess => _reports.IsSuccess;

        /// <summary>
        /// Gets the status of the smoke tests session report.
        /// </summary>
        public string Status => _reports.Status;

        /// <summary>
        /// Gets the list of the requested Smoke test categories.
        /// </summary>
        public string[] RequestedCategories { get; }

        /// <summary>
        /// Gets all the <see cref="SmokeTestResultWithMetaData"/> results of this Smoke test session.
        /// </summary>
        public SmokeTestsResults Results { get; }

        /// <summary>
        /// Gets the API instance identifier.
        /// </summary>
        public string InstanceIdentifier => _apiRuntimeDescription.InstanceIdentifier;

        /// <summary>
        /// Gets the API version.
        /// </summary>
        public string ApiVersion => _apiRuntimeDescription.ApiVersion;

        /// <summary>
        /// Gets the OS name of this API instance.
        /// </summary>
        public string OsName => _apiRuntimeDescription.OsName;

        /// <summary>
        /// Gets the Azure region name where this API instance is running.
        /// </summary>
        public string AzureRegionName => _apiRuntimeDescription.AzureRegionName;

        /// <summary>
        /// Gets the number of Processors this API instance has.
        /// </summary>
        public string NbOfProcessors => _apiRuntimeDescription.NbOfProcessors;
        
        /// <summary>
        /// Instantiates a <see cref="SmokeTestsSessionReportDto"/>.
        /// </summary>
        /// <param name="reports">The <see cref="SmokeTestsSessionReport"/> to be adapted.</param>
        /// <param name="apiRuntimeDescription">The <see cref="ApiRuntimeDescription"/> associated to that smoke test execution.</param>
        /// <param name="smokeTestResultWithMetaDataDtos">The <see cref="IEnumerable&lt;SmokeTestResultWithMetaDataDto&gt;"/> containing all the smoke tests results.</param>
        /// <param name="categories"></param>
        public SmokeTestsSessionReportDto(SmokeTestsSessionReport reports, ApiRuntimeDescription apiRuntimeDescription,
            IEnumerable<SmokeTestResultWithMetaDataDto> smokeTestResultWithMetaDataDtos, string[] categories)
        {
            _reports = reports;

            Results = new SmokeTestsResults(smokeTestResultWithMetaDataDtos.ToArray());

            _apiRuntimeDescription = apiRuntimeDescription;

            RequestedCategories = categories;
        }

        /// <summary>
        /// Instantiates a <see cref="SmokeTestsSessionReportDto"/>.
        /// </summary>
        /// <param name="apiRuntimeDescription">The <see cref="ApiRuntimeDescription"/> associated to that smoke test execution.</param>
        /// <param name="runtimeDescription"></param>
        public SmokeTestsSessionReportDto(ApiRuntimeDescription apiRuntimeDescription, string status = null)
        {
            status ??= string.Empty;
            _reports = new SmokeTestsSessionReport(status);

            _apiRuntimeDescription = apiRuntimeDescription;
            Results = new SmokeTestsResults(new SmokeTestResultWithMetaDataDto[0]);
        }
    }
}