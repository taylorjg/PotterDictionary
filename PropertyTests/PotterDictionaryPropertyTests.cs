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

        private static readonly Gen<string> GenOneTitle = Gen.elements(HarryPotterBookTitles);

        private static readonly IEnumerable<string[]> AllCombinationsOfTwoDifferentBooks =
            from title1 in HarryPotterBookTitles
            from title2 in HarryPotterBookTitles
            where title1 != title2
            select new[] { title1, title2 };

        private static readonly Gen<string[]> GenTwoDifferentTitles = Gen.elements(AllCombinationsOfTwoDifferentBooks);

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
        public void TwoBooksTheSame()
        {
            Spec
                .For(GenOneTitle, title => CheckPrice(Enumerable.Repeat(title, 2), 2 * UnitPrice))
                .QuickCheckThrowOnFailure();
        }

        [FsCheck.NUnit.Property]
        public void TwoBooksDifferent()
        {
            Spec
                .For(GenTwoDifferentTitles, titles => CheckPrice(titles, 2 * UnitPrice * 0.95m))
                .QuickCheckThrowOnFailure();
        }
    }
}
