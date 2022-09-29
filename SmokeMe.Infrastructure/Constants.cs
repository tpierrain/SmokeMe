﻿namespace SmokeMe
{
    /// <summary>
    /// Constants for the /smoke library
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Gets the name of the configuration key for the smoke test global execution timeout.
        /// </summary>
        public static string GlobaltimeoutinmsecConfigurationKey = "Smoke:GlobalTimeoutInMsec";

        /// <summary>
        /// Gets the name of the configuration key fo the global enabling of the lib.
        /// </summary>
        public static string IsEnabledConfigurationKey = "Smoke:IsSmokeTestExecutionEnabled";

        /// <summary>
        /// Gets the default value for the global timeout in milliseconds if the (<see cref="GlobaltimeoutinmsecConfigurationKey"/>) configuration key is not used to override it.
        /// </summary>
        public static int GlobalTimeoutInMsecDefaultValue = 30* 1000;
    }
}