using System.Collections.Generic;
using System.Linq;
using Code;
using FsCheck;
using FsCheck.Fluent;

namespace PropertyTests
{
    internal class PotterDictionaryPropertyTests
    {
        private const decimal UnitPrice = PotterCalculator.UnitPrice;

        private static bool TitlesAreDifferent(ICollection<string> titles)
        {
            return titles.Distinct().Count() == titles.Count;
        }

        private static readonly IEnumerable<string[]> CombinationsOfTwoDifferentTitles =
            from title1 in HarryPotterBooks.Titles
            from title2 in HarryPotterBooks.Titles
            let titles = new[] {title1, title2}
            where TitlesAreDifferent(titles)
            select titles;

        private static readonly IEnumerable<string[]> CombinationsOfThreeDifferentTitles =
            from title1 in HarryPotterBooks.Titles
            from title2 in HarryPotterBooks.Titles
            from title3 in HarryPotterBooks.Titles
            let titles = new[] {title1, title2, title3}
            where TitlesAreDifferent(titles)
            select titles;

        private static readonly IEnumerable<string[]> CombinationsOfFourDifferentTitles =
            from title1 in HarryPotterBooks.Titles
            from title2 in HarryPotterBooks.Titles
            from title3 in HarryPotterBooks.Titles
            from title4 in HarryPotterBooks.Titles
            let titles = new[] {title1, title2, title3, title4 }
            where TitlesAreDifferent(titles)
            select titles;

        private static readonly IEnumerable<string[]> CombinationsOfFiveDifferentTitles =
            from title1 in HarryPotterBooks.Titles
            from title2 in HarryPotterBooks.Titles
            from title3 in HarryPotterBooks.Titles
            from title4 in HarryPotterBooks.Titles
            from title5 in HarryPotterBooks.Titles
            let titles = new[] {title1, title2, title3, title4, title5 }
            where TitlesAreDifferent(titles)
            select titles;

        private static readonly Gen<string> GenOneTitle = Gen.elements(HarryPotterBooks.Titles);

        private static readonly Gen<string[]> GenMultipleTitlesTheSame =
            GenOneTitle.SelectMany(
                title => Gen.choose(1, 10).Select(
                    n => Enumerable.Repeat(title, n).ToArray()));

        private static readonly Gen<string[]> GenTwoDifferentTitles = Gen.elements(CombinationsOfTwoDifferentTitles);
        private static readonly Gen<string[]> GenThreeDifferentTitles = Gen.elements(CombinationsOfThreeDifferentTitles);
        private static readonly Gen<string[]> GenFourDifferentTitles = Gen.elements(CombinationsOfFourDifferentTitles);
        private static readonly Gen<string[]> GenFiveDifferentTitles = Gen.elements(CombinationsOfFiveDifferentTitles);

        // private static readonly Gen<string[]> GenFourTitles = Gen.arrayOfLength(4, Gen.elements(HarryPotterBooks.Titles));
        // private static readonly Gen<string[]> GenFourDifferentTitlesPlusTwoDifferentTitles =
        //     GenFourTitles.SelectMany(
        //         four => Gen.arrayOfLength(2, Gen.elements(four)).Select(
        //             two => four.Concat(two).ToArray()));

        private static readonly Gen<string[]> GenFourDifferentTitlesPlusTwoDifferentTitles =
            GenFourDifferentTitles.SelectMany(
                four =>
                    {
                        var v1 =
                            from title1 in four
                            from title2 in four
                            let titles = new[] {title1, title2}
                            where TitlesAreDifferent(titles)
                            select titles;
                        var v2 = Gen.elements(v1);
                        var v3 = v2.Select(two => four.Concat(two).ToArray());
                        return v3;
                    });

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
                .For(GenMultipleTitlesTheSame, titles => CheckPrice(titles, titles.Length * UnitPrice))
                .QuickCheckThrowOnFailure();
        }

        [FsCheck.NUnit.Property]
        public void TwoDifferentBooks()
        {
            Spec
                .For(GenTwoDifferentTitles, titles => CheckPrice(titles, 2 * UnitPrice * 0.95m))
                .QuickCheckThrowOnFailure();
        }

        [FsCheck.NUnit.Property]
        public void ThreeDifferentBooks()
        {
            Spec
                .For(GenThreeDifferentTitles, titles => CheckPrice(titles, 3 * UnitPrice * 0.90m))
                .QuickCheckThrowOnFailure();
        }

        [FsCheck.NUnit.Property]
        public void FourDifferentBooks()
        {
            Spec
                .For(GenFourDifferentTitles, titles => CheckPrice(titles, 4 * UnitPrice * 0.80m))
                .QuickCheckThrowOnFailure();
        }

        [FsCheck.NUnit.Property]
        public void FiveDifferentBooks()
        {
            Spec
                .For(GenFiveDifferentTitles, titles => CheckPrice(titles, 5 * UnitPrice * 0.75m))
                .QuickCheckThrowOnFailure();
        }

        [FsCheck.NUnit.Property]
        public void FourDifferentBooksPlusTwoDifferentBooks()
        {
            Spec
                .For(GenFourDifferentTitlesPlusTwoDifferentTitles, titles => CheckPrice(titles, (4 * UnitPrice * 0.80m) + (2 * UnitPrice * 0.95m)))
                .QuickCheckThrowOnFailure();
        }
    }
}
