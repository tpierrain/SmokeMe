using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;
using SmokeMe.Tests.SmokeTests;

namespace SmokeMe.Tests.Unit
{
    [TestFixture]
    public class SomeSmokeTestsForTestingPurposeShould
    {
        [Test]
        public async Task AlwaysPositiveSmokeTest_works_as_expected()
        {
            var delay = TimeSpan.FromMilliseconds(700);
            var smokeTest = new AlwaysPositiveSmokeTest(delay);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var executeScenario = await smokeTest.Scenario();
            stopwatch.Stop();

            Check.That((int)stopwatch.ElapsedMilliseconds).IsGreaterThan(Convert.ToInt32(delay.TotalMilliseconds));
        }
    }
}