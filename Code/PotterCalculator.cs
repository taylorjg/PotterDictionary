using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Code
{
    public class PotterCalculator
    {
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
            if (dictionary.Count == 1) return totalSoFar + dictionary.First().Value * 8m;

            var differentBooks = dictionary.Keys;

            Func<KeyValuePair<string, int>, KeyValuePair<string, int>> decrementBookCounts = kvp => new KeyValuePair<string, int>(kvp.Key, kvp.Value - 1);
            Func<KeyValuePair<string, int>, bool> eliminateZeroBookCounts = kvp => kvp.Value > 0;
            
            var newDictionary = dictionary
                .Select(decrementBookCounts)
                .Where(eliminateZeroBookCounts)
                .ToImmutableDictionary();

            var subTotal = totalSoFar + DiscountedPriceOfBooks(differentBooks);

            return CalculatePriceOfBooks(newDictionary, subTotal);
        }

        private static readonly ImmutableDictionary<int, decimal> NumDifferentBooksToDiscountPercentage = new Dictionary<int, decimal>
            {
                {2, 5},
                {3, 5},
                {4, 5},
                {5, 5}
            }.ToImmutableDictionary();

        private static decimal DiscountedPriceOfBooks(IEnumerable<string> differentBooks)
        {
            var numDifferentBooks = differentBooks.Count();
            var subTotalBeforeDiscount = numDifferentBooks * 8m;
            var discountPercentage = NumDifferentBooksToDiscountPercentage.GetValueOrDefault(numDifferentBooks, 0m);
            return (subTotalBeforeDiscount / 100) * (100 - discountPercentage);
        }
    }
}
