namespace SmokeMe.Infra
{
    /// <summary>
    /// Report for a smoke test execution that could not happened due to configuration settings.
    /// </summary>
    public class SmokeTestsDisabledReportDto : SmokeTestsSessionReportDto
    {
        public SmokeTestsDisabledReportDto(ApiRuntimeDescription apiRuntimeDescription) : base(apiRuntimeDescription, $"Smoke tests execution not enabled. Set the '{Constants.IsEnabledConfigurationKey}' configuration key to true if you want to enable it.")
        {
        }
    }
}