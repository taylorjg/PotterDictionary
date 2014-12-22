using System.Collections.Generic;
using System.Linq;
using Code;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    class PotterDictionaryTests
    {
        private const decimal UnitPrice = PotterCalculator.UnitPrice;

        private static IEnumerable<string> SplitCommaSeparatedTitles(string commaSeparatedTitles)
        {
            return commaSeparatedTitles
                .Split(',')
                .Select(title => title.Trim())
                .Where(title => title.Length > 0);
        }

        private static void CheckPrice(string commaSeparatedTitles, decimal expectedPrice)
        {
            var titles = SplitCommaSeparatedTitles(commaSeparatedTitles).ToArray();
            var actualPrice = PotterCalculator.CalculatePrice(titles);
            Assert.That(actualPrice, Is.EqualTo(expectedPrice));
        }

        [TestCaseSource("TestCaseSourceForBasics")]
        public void Basics(string commaSeparatedTitles, decimal expectedPrice)
        {
            CheckPrice(commaSeparatedTitles, expectedPrice);
        }

        [TestCaseSource("TestCaseSourceForSimpleDiscounts")]
        public void SimpleDiscounts(string commaSeparatedTitles, decimal expectedPrice)
        {
            CheckPrice(commaSeparatedTitles, expectedPrice);
        }

        [TestCaseSource("TestCaseSourceForSeveralDiscounts")]
        public void SeveralDiscounts(string commaSeparatedTitles, decimal expectedPrice)
        {
            CheckPrice(commaSeparatedTitles, expectedPrice);
        }

        // ReSharper disable UnusedMethodReturnValue.Local

        private static IEnumerable<ITestCaseData> TestCaseSourceForBasics()
        {
            yield return new TestCaseData("", 0 * UnitPrice);
            yield return new TestCaseData("A", 1 * UnitPrice);
            yield return new TestCaseData("B", 1 * UnitPrice);
            yield return new TestCaseData("C", 1 * UnitPrice);
            yield return new TestCaseData("D", 1 * UnitPrice);
            yield return new TestCaseData("E", 1 * UnitPrice);
            yield return new TestCaseData("A,A", 2 * UnitPrice);
            yield return new TestCaseData("B,B,B", 3 * UnitPrice);
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForSimpleDiscounts()
        {
            yield return new TestCaseData("A,B", 2 * UnitPrice * 0.95m);
            yield return new TestCaseData("A,C,E", 3 * UnitPrice * 0.90m);
            yield return new TestCaseData("A,B,C,E", 4 * UnitPrice * 0.80m);
            yield return new TestCaseData("A,B,C,D,E", 5 * UnitPrice * 0.75m);
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForSeveralDiscounts()
        {
            yield return new TestCaseData("A,A,B", (1 * UnitPrice) + (2 * UnitPrice * 0.95m));
            yield return new TestCaseData("A,A,B,B", (2 * UnitPrice * 0.95m) * 2);
            yield return new TestCaseData("A,A,B,C,C,D", (4 * UnitPrice * 0.80m) + (2 * UnitPrice * 0.95m));
            yield return new TestCaseData("A,B,B,C,D,E", (1 * UnitPrice) + (5 * UnitPrice * 0.75m));
        }
    }
}
