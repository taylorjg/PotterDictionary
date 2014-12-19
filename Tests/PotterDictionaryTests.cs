using Code;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    class PotterDictionaryTests
    {
        [Test]
        public void OneBook()
        {
            var bookA = new Book("A");
            var actual = PotterCalculator.Calculate(bookA);
            Assert.That(actual, Is.EqualTo(8m));
        }

        [Test]
        public void TwoBooksSame()
        {
            var bookA = new Book("A");
            var actual = PotterCalculator.Calculate(bookA, bookA);
            Assert.That(actual, Is.EqualTo(16m));
        }

        [Test]
        public void TwoBooksDifferent()
        {
            var bookA = new Book("A");
            var bookB = new Book("B");
            var actual = PotterCalculator.Calculate(bookA, bookB);
            Assert.That(actual, Is.EqualTo(16m * 0.95m));
        }

        [Test]
        public void TwoBooksDifferentPlusOneSame()
        {
            var bookA = new Book("A");
            var bookB = new Book("B");
            var actual = PotterCalculator.Calculate(bookA, bookA, bookB);
            Assert.That(actual, Is.EqualTo(8m + (16m * 0.95m)));
        }
    }
}
