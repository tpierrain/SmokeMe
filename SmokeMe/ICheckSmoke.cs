using System;

namespace SmokeMe
{
    /// <summary>
    /// The ICheckSmoke interface is deprecated and MUST be replaced by SmokeTest abstract class. To do so, just replace all reference to ICheckSmoke with SmokeTest and add the 'override' keyword to your existing SmokeTestName, Description properties, but also to the Scenario() method which is now an abstract method.
    /// </summary>
    [Obsolete("The ICheckSmoke interface is deprecated and MUST be replaced by SmokeTest abstract class. To do so, just replace all reference to ICheckSmoke with SmokeTest and add the 'override' keyword to your existing SmokeTestName, Description properties, but also to the Scenario() method which is now an abstract method")]
    public interface ICheckSmoke
    {
        /// <summary>
        /// Breaking change: the ICheckSmoke interface is deprecated and MUST be replaced by SmokeTest abstract class. To do so, just replace all reference to ICheckSmoke with SmokeTest and add the 'override' keyword to your existing SmokeTestName, Description properties, but also to the Scenario() method which is now an abstract method.
        /// </summary>
        void WithTheV2MajorBreakingChangeOfSmokeMeYouMustReplaceAllYourPreviousReferenceToICheckSmokeInterfaceWithTheSmokeTestAbstractClass();
    }
}