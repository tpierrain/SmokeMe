using System;
using NFluent;
using NUnit.Framework;
using SmokeMe.Helpers;
using SmokeMe.Tests.SmokeTests;

namespace SmokeMe.Tests.Unit
{
    [TestFixture]
    public class SmokeTestResultWithMetaDataShould
    {
        [Test]
        public void Default_value_for_Status_is_Executed_when_not_provided_to_the_constructor_and_discared_is_set_to_false()
        {
            var smokeTest = new AlwaysPositiveSmokeTest(TimeSpan.FromMilliseconds(1));

            Status? status = null;
            bool? discarded = false;

            var smokeTestResultWithMetaData = new SmokeTestResultWithMetaData(new SmokeTestResult(true), TimeSpan.Zero, new SmokeTestInstanceWithMetaData(smokeTest, smokeTest.GetCategories()), smokeTest.GetType().FullName, discarded: discarded, status);

            Check.That(smokeTestResultWithMetaData.Status).IsEqualTo(Status.Executed);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void Keep_the_Ignored_Status_Whatever_the_provided_Value_for_discarded(bool discarded)
        {
            var smokeTest = new AlwaysPositiveSmokeTest(TimeSpan.FromMilliseconds(1));

            var status = Status.Ignored;

            var smokeTestResultWithMetaData = new SmokeTestResultWithMetaData(new SmokeTestResult(true), TimeSpan.Zero, new SmokeTestInstanceWithMetaData(smokeTest, smokeTest.GetCategories()), smokeTest.GetType().FullName, discarded: discarded, status);

            Check.That(smokeTestResultWithMetaData.Status).IsEqualTo(Status.Ignored);
        }

        [TestCase(Status.Executed)]
        [TestCase(Status.Timeout)]
        [TestCase(Status.Discarded)]
        public void Discarded_Status_is_automatically_set_if_discarded_is_set_to_true_whatever_the_provided_Status_as_soon_as_it_is_not_Ignored(Status providedStatusThatIsNotIgnored)
        {
            var smokeTest = new AlwaysPositiveSmokeTest(TimeSpan.FromMilliseconds(1));

            bool? discarded = true;

            var smokeTestResultWithMetaData = new SmokeTestResultWithMetaData(new SmokeTestResult(true), TimeSpan.Zero, new SmokeTestInstanceWithMetaData(smokeTest, smokeTest.GetCategories()), smokeTest.GetType().FullName, discarded: discarded, providedStatusThatIsNotIgnored);

            Check.That(smokeTestResultWithMetaData.Status).IsEqualTo(Status.Discarded);
        }
        
    }
}