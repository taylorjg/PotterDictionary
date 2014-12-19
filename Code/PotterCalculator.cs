using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Code
{
    public class PotterCalculator
    {
        public const decimal UnitPrice = 8m;

        public static decimal Calculate(params Book[] books)
        {
            return CalculatePriceOfBooks(MakeDictionary(books), 0m);
        }

        private static ImmutableDictionary<string, int> MakeDictionary(IEnumerable<Book> books)
        {
            return books
                .GroupBy(b => b.Name)
                .ToImmutableDictionary(g => g.Key, g => g.Count());
        }

        private static decimal CalculatePriceOfBooks(ImmutableDictionary<string, int> dictionary, decimal totalSoFar)
        {
            if (dictionary.Count == 0) return totalSoFar;
            if (dictionary.Count == 1) return totalSoFar + UndiscountedPriceOfBooks(dictionary.First());

            var differentBooks = dictionary.Keys;

            Func<KeyValuePair<string, int>, KeyValuePair<string, int>> decrementBookCount = kvp => new KeyValuePair<string, int>(kvp.Key, kvp.Value - 1);
            Func<KeyValuePair<string, int>, bool> bookCountIsPositive = kvp => kvp.Value > 0;
            
            var reducedDictionary = dictionary
                .Select(decrementBookCount)
                .Where(bookCountIsPositive)
                .ToImmutableDictionary();

            var subTotal = totalSoFar + DiscountedPriceOfBooks(differentBooks);

            return CalculatePriceOfBooks(reducedDictionary, subTotal);
        }

        private static decimal UndiscountedPriceOfBooks(KeyValuePair<string, int> kvp)
        {
            return kvp.Value * UnitPrice;
        }

        private static decimal DiscountedPriceOfBooks(IEnumerable<string> differentBooks)
        {
            var numDifferentBooks = differentBooks.Count();
            var subTotalBeforeDiscount = numDifferentBooks * UnitPrice;
            var discountPercentage = NumberOfDifferentBooksToDiscountPercentage.GetValueOrDefault(numDifferentBooks, 0m);
            return (subTotalBeforeDiscount / 100) * (100 - discountPercentage);
        }

        private static readonly ImmutableDictionary<int, decimal> NumberOfDifferentBooksToDiscountPercentage = new Dictionary<int, decimal>
            {
                {2, 5},
                {3, 10},
                {4, 20},
                {5, 25}
            }.ToImmutableDictionary();
    }
}
