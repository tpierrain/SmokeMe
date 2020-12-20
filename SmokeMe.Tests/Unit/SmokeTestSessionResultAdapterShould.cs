using System;
using NFluent;
using NUnit.Framework;
using SmokeMe.Controllers;

namespace SmokeMe.Tests.Unit
{
    [TestFixture]
    [SetCulture("fr-fr")]
    public class SmokeTestSessionResultAdapterShould
    {
        [Test]
        public void Adapt_using_second_unit_when_duration_is_equal_or_more_than_a_second()
        {

            var adaptation = SmokeTestSessionResultAdapter.AdaptDurationToMakeItReadable(TimeSpan.FromMilliseconds(1200));
            Check.That(adaptation).IsEqualTo("1.2 seconds");

            adaptation = SmokeTestSessionResultAdapter.AdaptDurationToMakeItReadable(TimeSpan.FromMilliseconds(1000));
            Check.That(adaptation).IsEqualTo("1 second");

            adaptation = SmokeTestSessionResultAdapter.AdaptDurationToMakeItReadable(TimeSpan.FromMilliseconds(2000));
            Check.That(adaptation).IsEqualTo("2 seconds");
        }

        [Test]
        public void Adapt_rounding_seconds_when_needed()
        {
            var adaptation = SmokeTestSessionResultAdapter.AdaptDurationToMakeItReadable(TimeSpan.FromSeconds(1.002));
            Check.That(adaptation).IsEqualTo("1 second");

            adaptation = SmokeTestSessionResultAdapter.AdaptDurationToMakeItReadable(TimeSpan.FromSeconds(1.2006765));
            Check.That(adaptation).IsEqualTo("1.2 seconds");
        }

        [Test]
        public void Adapt_using_millisecond_unit_when_duration_is_strictly_below_a_second()
        {
            var adaptation = SmokeTestSessionResultAdapter.AdaptDurationToMakeItReadable(TimeSpan.FromMilliseconds(999));
            Check.That(adaptation).IsEqualTo("999 milliseconds");

            adaptation = SmokeTestSessionResultAdapter.AdaptDurationToMakeItReadable(TimeSpan.FromMilliseconds(30));
            Check.That(adaptation).IsEqualTo("30 milliseconds");

            adaptation = SmokeTestSessionResultAdapter.AdaptDurationToMakeItReadable(TimeSpan.FromMilliseconds(1));
            Check.That(adaptation).IsEqualTo("1 millisecond");

            adaptation = SmokeTestSessionResultAdapter.AdaptDurationToMakeItReadable(TimeSpan.FromMilliseconds(2));
            Check.That(adaptation).IsEqualTo("2 milliseconds");
        }

        [Test]
        public void Adapt_rounding_milliseconds_when_needed()
        {
            var adaptation = SmokeTestSessionResultAdapter.AdaptDurationToMakeItReadable(TimeSpan.FromMilliseconds(500.999));
            Check.That(adaptation).IsEqualTo("501 milliseconds");
        }
    }
}