using System.Collections.Generic;
using System.Linq;
using Code;
using FsCheck;
using FsCheck.Fluent;
using NUnit.Framework;

namespace PropertyTests
{
    internal class PotterDictionaryPropertyTests
    {
        private const decimal UnitPrice = PotterCalculator.UnitPrice;

        private static bool TitlesAreDifferent(ICollection<string> titles)
        {
            return titles.Distinct().Count() == titles.Count;
        }

        private static readonly IEnumerable<string[]> CombinationsOfTwoDifferentBooks =
            from title1 in HarryPotterBooks.Titles
            from title2 in HarryPotterBooks.Titles
            let titles = new[] {title1, title2}
            where TitlesAreDifferent(titles)
            select titles;

        private static readonly IEnumerable<string[]> CombinationsOfThreeDifferentBooks =
            from title1 in HarryPotterBooks.Titles
            from title2 in HarryPotterBooks.Titles
            from title3 in HarryPotterBooks.Titles
            let titles = new[] {title1, title2, title3}
            where TitlesAreDifferent(titles)
            select titles;

        private static readonly IEnumerable<string[]> CombinationsOfFourDifferentBooks =
            from title1 in HarryPotterBooks.Titles
            from title2 in HarryPotterBooks.Titles
            from title3 in HarryPotterBooks.Titles
            from title4 in HarryPotterBooks.Titles
            let titles = new[] {title1, title2, title3, title4 }
            where TitlesAreDifferent(titles)
            select titles;

        private static readonly IEnumerable<string[]> CombinationsOfFiveDifferentBooks =
            from title1 in HarryPotterBooks.Titles
            from title2 in HarryPotterBooks.Titles
            from title3 in HarryPotterBooks.Titles
            from title4 in HarryPotterBooks.Titles
            from title5 in HarryPotterBooks.Titles
            let titles = new[] {title1, title2, title3, title4, title5 }
            where TitlesAreDifferent(titles)
            select titles;

        private static readonly Gen<string> GenOneTitle = Gen.elements(HarryPotterBooks.Titles);

        private static readonly Gen<string[]> GenMultipleBooksTheSame =
            GenOneTitle.SelectMany(
                title => Gen.choose(1, 10).Select(
                    n => Enumerable.Repeat(title, n).ToArray()));

        private static readonly Gen<string[]> GenTwoDifferentTitles = Gen.elements(CombinationsOfTwoDifferentBooks);
        private static readonly Gen<string[]> GenThreeDifferentTitles = Gen.elements(CombinationsOfThreeDifferentBooks);
        private static readonly Gen<string[]> GenFourDifferentTitles = Gen.elements(CombinationsOfFourDifferentBooks);
        private static readonly Gen<string[]> GenFiveDifferentTitles = Gen.elements(CombinationsOfFiveDifferentBooks);

        private static bool CheckPrice(IEnumerable<string> titles, decimal expectedPrice)
        {
            var books = titles.Select(title => new Book(title)).ToArray();
            var actualPrice = PotterCalculator.CalculatePrice(books);
            return actualPrice == expectedPrice;
        }

        private static bool CheckPrice(string title, decimal expectedPrice)
        {
            return CheckPrice(new[] {title}, expectedPrice);
        }

        [SetUp]
        public void SetUp()
        {
        }

        [FsCheck.NUnit.Property]
        public void OneBook()
        {
            Spec
                .For(GenOneTitle, title => CheckPrice(title, UnitPrice))
                .QuickCheckThrowOnFailure();
        }

        [FsCheck.NUnit.Property]
        public void MultipleBooksTheSame()
        {
            Spec
                .For(GenMultipleBooksTheSame, titles => CheckPrice(titles, titles.Length * UnitPrice))
                .QuickCheckThrowOnFailure();
        }

        [FsCheck.NUnit.Property]
        public void TwoBooksDifferent()
        {
            Spec
                .For(GenTwoDifferentTitles, titles => CheckPrice(titles, 2 * UnitPrice * 0.95m))
                .QuickCheckThrowOnFailure();
        }

        [FsCheck.NUnit.Property]
        public void ThreeBooksDifferent()
        {
            Spec
                .For(GenThreeDifferentTitles, titles => CheckPrice(titles, 3 * UnitPrice * 0.90m))
                .QuickCheckThrowOnFailure();
        }

        [FsCheck.NUnit.Property]
        public void FourBooksDifferent()
        {
            Spec
                .For(GenFourDifferentTitles, titles => CheckPrice(titles, 4 * UnitPrice * 0.80m))
                .QuickCheckThrowOnFailure();
        }

        [FsCheck.NUnit.Property]
        public void FiveBooksDifferent()
        {
            Spec
                .For(GenFiveDifferentTitles, titles => CheckPrice(titles, 5 * UnitPrice * 0.75m))
                .QuickCheckThrowOnFailure();
        }
    }
}
