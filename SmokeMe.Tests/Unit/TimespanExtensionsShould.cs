using System;
using NFluent;
using NUnit.Framework;
using SmokeMe.Helpers;

namespace SmokeMe.Tests.Unit
{
    [TestFixture]
    [SetCulture("fr-fr")]
    public class TimespanExtensionsShould
    {
        [Test]
        public void Adapt_using_second_unit_when_duration_is_equal_or_more_than_a_second()
        {

            var adaptation = TimeSpan.FromMilliseconds(1200).GetHumanReadableVersion();
            Check.That(adaptation).IsEqualTo("1.2 seconds");

            adaptation = TimeSpan.FromMilliseconds(1000).GetHumanReadableVersion();
            Check.That(adaptation).IsEqualTo("1 second");

            adaptation = TimeSpan.FromMilliseconds(2000).GetHumanReadableVersion();
            Check.That(adaptation).IsEqualTo("2 seconds");
        }

        [Test]
        public void Adapt_rounding_seconds_when_needed()
        {
            var adaptation = TimeSpan.FromSeconds(1.002).GetHumanReadableVersion();
            Check.That(adaptation).IsEqualTo("1 second");

            adaptation = TimeSpan.FromSeconds(1.2006765).GetHumanReadableVersion();
            Check.That(adaptation).IsEqualTo("1.2 seconds");
        }

        [Test]
        public void Adapt_using_millisecond_unit_when_duration_is_strictly_below_a_second()
        {
            var adaptation = TimeSpan.FromMilliseconds(999).GetHumanReadableVersion();
            Check.That(adaptation).IsEqualTo("999 milliseconds");

            adaptation = TimeSpan.FromMilliseconds(30).GetHumanReadableVersion();
            Check.That(adaptation).IsEqualTo("30 milliseconds");

            adaptation = TimeSpan.FromMilliseconds(1).GetHumanReadableVersion();
            Check.That(adaptation).IsEqualTo("1 millisecond");

            adaptation = TimeSpan.FromMilliseconds(2).GetHumanReadableVersion();
            Check.That(adaptation).IsEqualTo("2 milliseconds");
        }

        [Test]
        public void Adapt_rounding_milliseconds_when_needed()
        {
            var adaptation = TimeSpan.FromMilliseconds(500.999).GetHumanReadableVersion();
            Check.That(adaptation).IsEqualTo("501 milliseconds");
        }

        [Test]
        public void Adapt_rounding_microseconds_when_needed()
        {
            var adaptation = TimeSpan.FromMilliseconds(0.392).GetHumanReadableVersion();
            Check.That(adaptation).IsEqualTo("392 microseconds");
            // 0.392 ms
        }
    }
}