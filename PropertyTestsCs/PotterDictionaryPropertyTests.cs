﻿using System;
using System.Collections.Generic;
using System.Linq;
using Code;
using FsCheck;
using FsCheck.Fluent;
using Microsoft.FSharp.Core;

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

        private static void CheckPriceOfBooksSpec(Gen<string[]> gen, Func<string[], decimal> expectedPriceFunc)
        {
            Spec
                .For(gen, titles => CheckPriceOfBooks(titles, expectedPriceFunc(titles)))
                .Shrink(_ => Enumerable.Empty<string[]>())
                .QuickCheckThrowOnFailure();
        }

        private static void CheckPriceOfBooksSpecWithFixedPrice(Gen<string[]> gen, decimal expectedPrice)
        {
            CheckPriceOfBooksSpec(gen, _ => expectedPrice);
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
        private static readonly FSharpFunc<string, string[]> StringToSingletonArray = FSharpFunc<string, string[]>.FromConverter(s => new[] {s});
        private static readonly Gen<string[]> GenOneTitleArray = Gen.map(StringToSingletonArray, GenOneTitle);
        private static readonly Gen<string[]> GenTwoDifferentTitles = Gen.elements(CombinationsOfTwoDifferentTitles(HarryPotterBooks.Titles));
        private static readonly Gen<string[]> GenThreeDifferentTitles = Gen.elements(CombinationsOfThreeDifferentTitles(HarryPotterBooks.Titles));
        private static readonly Gen<string[]> GenFourDifferentTitles = Gen.elements(CombinationsOfFourDifferentTitles(HarryPotterBooks.Titles));
        private static readonly Gen<string[]> GenFiveDifferentTitles = Gen.elements(CombinationsOfFiveDifferentTitles(HarryPotterBooks.Titles));

        private static readonly Gen<string[]> GenMultipleTitlesTheSame =
            from title in GenOneTitle
            from n in Gen.choose(0, 20)
            select Enumerable.Repeat(title, n).ToArray();

        private static readonly Gen<string[]> GenOverlappingFourDifferentTitlesPlusTwoDifferentTitles =
            from four in GenFourDifferentTitles
            from two in Gen.elements(CombinationsOfTwoDifferentTitles(four))
            select four.Concat(two).ToArray();

        [FsCheck.NUnit.Property]
        public void OneBook()
        {
            const decimal expectedPrice = UnitPrice;
            CheckPriceOfBooksSpecWithFixedPrice(GenOneTitleArray, expectedPrice);
        }

        [FsCheck.NUnit.Property]
        public void MultipleBooksTheSame()
        {
            CheckPriceOfBooksSpec(GenMultipleTitlesTheSame, titles => titles.Length * UnitPrice);
        }

        [FsCheck.NUnit.Property]
        public void TwoDifferentBooks()
        {
            const decimal expectedPrice = 2 * UnitPrice * 0.95m;
            CheckPriceOfBooksSpecWithFixedPrice(GenTwoDifferentTitles, expectedPrice);
        }

        [FsCheck.NUnit.Property]
        public void ThreeDifferentBooks()
        {
            const decimal expectedPrice = 3 * UnitPrice * 0.90m;
            CheckPriceOfBooksSpecWithFixedPrice(GenThreeDifferentTitles, expectedPrice);
        }

        [FsCheck.NUnit.Property]
        public void FourDifferentBooks()
        {
            const decimal expectedPrice = 4 * UnitPrice * 0.80m;
            CheckPriceOfBooksSpecWithFixedPrice(GenFourDifferentTitles, expectedPrice);
        }

        [FsCheck.NUnit.Property]
        public void FiveDifferentBooks()
        {
            const decimal expectedPrice = 5 * UnitPrice * 0.75m;
            CheckPriceOfBooksSpecWithFixedPrice(GenFiveDifferentTitles, expectedPrice);
        }

        [FsCheck.NUnit.Property]
        public void OverlappingFourDifferentBooksPlusTwoDifferentBooks()
        {
            var gen = GenOverlappingFourDifferentTitlesPlusTwoDifferentTitles;
            const decimal expectedPrice = (4 * UnitPrice * 0.80m) + (2 * UnitPrice * 0.95m);
            CheckPriceOfBooksSpecWithFixedPrice(gen, expectedPrice);
        }
    }
}