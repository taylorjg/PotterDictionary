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

        private static readonly string[] HarryPotterBookTitles = new[]
            {
                "Harry Potter and the Philosopher's Stone",
                "Harry Potter and the Chamber of Secrets",
                "Harry Potter and the Prisoner of Azkaban",
                "Harry Potter and the Goblet of Fire",
                "Harry Potter and the Order of the Phoenix"
            };

        private static readonly IEnumerable<string[]> AllCombinationsOfTwoDifferentBooks =
            from title1 in HarryPotterBookTitles
            from title2 in HarryPotterBookTitles
            let titles = new[] {title1, title2}
            where titles.Distinct().Count() == titles.Length
            select titles;

        private static readonly IEnumerable<string[]> AllCombinationsOfThreeDifferentBooks =
            from title1 in HarryPotterBookTitles
            from title2 in HarryPotterBookTitles
            from title3 in HarryPotterBookTitles
            let titles = new[] {title1, title2, title3}
            where titles.Distinct().Count() == titles.Length
            select titles;

        private static readonly IEnumerable<string[]> AllCombinationsOfFourDifferentBooks =
            from title1 in HarryPotterBookTitles
            from title2 in HarryPotterBookTitles
            from title3 in HarryPotterBookTitles
            from title4 in HarryPotterBookTitles
            let titles = new[] {title1, title2, title3, title4 }
            where titles.Distinct().Count() == titles.Length
            select titles;

        private static readonly IEnumerable<string[]> AllCombinationsOfFiveDifferentBooks =
            from title1 in HarryPotterBookTitles
            from title2 in HarryPotterBookTitles
            from title3 in HarryPotterBookTitles
            from title4 in HarryPotterBookTitles
            from title5 in HarryPotterBookTitles
            let titles = new[] {title1, title2, title3, title4, title5 }
            where titles.Distinct().Count() == titles.Length
            select titles;

        private static readonly Gen<string> GenOneTitle = Gen.elements(HarryPotterBookTitles);

        private static readonly Gen<string[]> GenMultipleBooksTheSame =
            GenOneTitle.SelectMany(
                title => Gen.choose(1, 10).Select(
                    n => Enumerable.Repeat(title, n).ToArray()));

        private static readonly Gen<string[]> GenTwoDifferentTitles = Gen.elements(AllCombinationsOfTwoDifferentBooks);
        private static readonly Gen<string[]> GenThreeDifferentTitles = Gen.elements(AllCombinationsOfThreeDifferentBooks);
        private static readonly Gen<string[]> GenFourDifferentTitles = Gen.elements(AllCombinationsOfFourDifferentBooks);
        private static readonly Gen<string[]> GenFiveDifferentTitles = Gen.elements(AllCombinationsOfFiveDifferentBooks);

        private static bool CheckPrice(IEnumerable<string> titles, decimal expectedPrice)
        {
            var actualPrice = PotterCalculator.CalculatePrice(titles.Select(title => new Book(title)).ToArray());
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
