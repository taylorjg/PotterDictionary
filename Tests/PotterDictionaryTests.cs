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

        [Test]
        public void OneBook()
        {
            var bookA = new Book("A");
            var actual = PotterCalculator.CalculatePrice(bookA);
            Assert.That(actual, Is.EqualTo(UnitPrice));
        }

        [Test]
        public void TwoBooksSame()
        {
            var bookA = new Book("A");
            var actual = PotterCalculator.CalculatePrice(bookA, bookA);
            Assert.That(actual, Is.EqualTo(2 * UnitPrice));
        }

        [Test]
        public void TwoBooksDifferent()
        {
            var bookA = new Book("A");
            var bookB = new Book("B");
            var actual = PotterCalculator.CalculatePrice(bookA, bookB);
            Assert.That(actual, Is.EqualTo(2 * UnitPrice * 0.95m));
        }

        [TestCaseSource("TestCaseSourceForTwoBooksSamePlusOneDifferent")]
        public void TwoBooksSamePlusOneDifferent(string bookNames, decimal expected)
        {
            var actual = PotterCalculator.CalculatePrice(bookNames.ToBooks().ToArray());
            Assert.That(actual, Is.EqualTo(expected));
        }

        // ReSharper disable UnusedMethodReturnValue.Local

        private static IEnumerable<ITestCaseData> TestCaseSourceForTwoBooksSamePlusOneDifferent()
        {
            const decimal expected = UnitPrice + (2m * UnitPrice * 0.95m);

            yield return new TestCaseData("A,A,B", expected);
            yield return new TestCaseData("A,B,A", expected);
            yield return new TestCaseData("B,A,A", expected);

            yield return new TestCaseData("A,A,C", expected);
            yield return new TestCaseData("A,C,A", expected);
            yield return new TestCaseData("C,A,A", expected);

            yield return new TestCaseData("E,E,D", expected);
            yield return new TestCaseData("E,D,E", expected);
            yield return new TestCaseData("D,E,E", expected);
        }
    }
}
