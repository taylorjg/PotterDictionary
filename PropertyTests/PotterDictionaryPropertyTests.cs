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

        private static bool CheckPriceOfBooks(string[] titles, decimal expectedPrice)
        {
            var actualPrice = PotterCalculator.CalculatePrice(titles);
            return actualPrice == expectedPrice;
        }

        private static bool CheckPriceOfBooks(string title, decimal expectedPrice)
        {
            return CheckPriceOfBooks(new[] { title }, expectedPrice);
        }

        private static bool TitlesAreDifferent(ICollection<string> titles)
        {
            return titles.Distinct().Count() == titles.Count;
        }

        private static IEnumerable<string[]> CombinationsOfTwoDifferentTitles(string[] allTitles)
        {
            return from title1 in allTitles
                   from title2 in allTitles
                   let titles = new[] { title1, title2 }
                   where TitlesAreDifferent(titles)
                   select titles;
        }

        private static IEnumerable<string[]> CombinationsOfThreeDifferentTitles(string[] allTitles)
        {
            return from title1 in allTitles
                   from title2 in allTitles
                   from title3 in allTitles
                   let titles = new[] { title1, title2, title3 }
                   where TitlesAreDifferent(titles)
                   select titles;
        }

        private static IEnumerable<string[]> CombinationsOfFourDifferentTitles(string[] allTitles)
        {
            return from title1 in allTitles
                   from title2 in allTitles
                   from title3 in allTitles
                   from title4 in allTitles
                   let titles = new[] { title1, title2, title3, title4 }
                   where TitlesAreDifferent(titles)
                   select titles;
        }

        private static IEnumerable<string[]> CombinationsOfFiveDifferentTitles(string[] allTitles)
        {
            return from title1 in allTitles
                   from title2 in allTitles
                   from title3 in allTitles
                   from title4 in allTitles
                   from title5 in allTitles
                   let titles = new[] { title1, title2, title3, title4, title5 }
                   where TitlesAreDifferent(titles)
                   select titles;
        }

        private static readonly Gen<string> GenOneTitle = Gen.elements(HarryPotterBooks.Titles);
        private static readonly Gen<string[]> GenTwoDifferentTitles = Gen.elements(CombinationsOfTwoDifferentTitles(HarryPotterBooks.Titles));
        private static readonly Gen<string[]> GenThreeDifferentTitles = Gen.elements(CombinationsOfThreeDifferentTitles(HarryPotterBooks.Titles));
        private static readonly Gen<string[]> GenFourDifferentTitles = Gen.elements(CombinationsOfFourDifferentTitles(HarryPotterBooks.Titles));
        private static readonly Gen<string[]> GenFiveDifferentTitles = Gen.elements(CombinationsOfFiveDifferentTitles(HarryPotterBooks.Titles));

        private static readonly Gen<string[]> GenMultipleTitlesTheSame =
            from title in GenOneTitle
            from n in Gen.choose(1, 10)
            select Enumerable.Repeat(title, n).ToArray();

        private static readonly Gen<string[]> GenOverlappingFourDifferentTitlesPlusTwoDifferentTitles =
            from four in GenFourDifferentTitles
            from two in Gen.elements(CombinationsOfTwoDifferentTitles(four))
            select four.Concat(two).ToArray();

        private class NonShrinkStringArray : Arbitrary<string[]>
        {
            public override Gen<string[]> Generator
            {
                get { return Gen.constant(new string[0]); }
            }

            public override IEnumerable<string[]> Shrinker(string[] _)
            {
                return Enumerable.Empty<string[]>();
            }
        }

        // ReSharper disable ClassNeverInstantiated.Local
        private class MyArbitraries
        {
            // ReSharper disable UnusedMember.Local
            public static Arbitrary<string[]> StringArray()
            {
                return new NonShrinkStringArray();
            }
            // ReSharper restore UnusedMember.Local
        }
        // ReSharper restore ClassNeverInstantiated.Local

        [SetUp]
        public void SetUp()
        {
            // We never want to shrink our string arrays because the exact contents of our
            // generated string arrays are carefully calculated. There may be a better way
            // to achieve this but for now, register an Arbitrary<string[]> whose Shrinker
            // returns an empty enumerable. There is something called DontShrink but I
            // don't know how to use it.
            // https://fsharp.github.io/FsCheck/reference/fscheck-dontshrink-1.html
            DefaultArbitraries.Add<MyArbitraries>();
        }

        [FsCheck.NUnit.Property]
        public void OneBook()
        {
            Spec
                .For(GenOneTitle, title => CheckPriceOfBooks(title, UnitPrice))
                .QuickCheckThrowOnFailure();
        }

        [FsCheck.NUnit.Property]
        public void MultipleBooksTheSame()
        {
            Spec
                .For(GenMultipleTitlesTheSame, titles => CheckPriceOfBooks(titles, titles.Length * UnitPrice))
                .QuickCheckThrowOnFailure();
        }

        [FsCheck.NUnit.Property]
        public void TwoDifferentBooks()
        {
            Spec
                .For(GenTwoDifferentTitles, titles => CheckPriceOfBooks(titles, 2 * UnitPrice * 0.95m))
                .QuickCheckThrowOnFailure();
        }

        [FsCheck.NUnit.Property]
        public void ThreeDifferentBooks()
        {
            Spec
                .For(GenThreeDifferentTitles, titles => CheckPriceOfBooks(titles, 3 * UnitPrice * 0.90m))
                .QuickCheckThrowOnFailure();
        }

        [FsCheck.NUnit.Property]
        public void FourDifferentBooks()
        {
            Spec
                .For(GenFourDifferentTitles, titles => CheckPriceOfBooks(titles, 4 * UnitPrice * 0.80m))
                .QuickCheckThrowOnFailure();
        }

        [FsCheck.NUnit.Property]
        public void FiveDifferentBooks()
        {
            Spec
                .For(GenFiveDifferentTitles, titles => CheckPriceOfBooks(titles, 5 * UnitPrice * 0.75m))
                .QuickCheckThrowOnFailure();
        }

        [FsCheck.NUnit.Property]
        public void OverlappingFourDifferentBooksPlusTwoDifferentBooks()
        {
            Spec
                .For(GenOverlappingFourDifferentTitlesPlusTwoDifferentTitles, titles => CheckPriceOfBooks(titles, (4 * UnitPrice * 0.80m) + (2 * UnitPrice * 0.95m)))
                .QuickCheckThrowOnFailure();
        }
    }
}
