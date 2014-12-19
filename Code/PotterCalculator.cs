using System;
using System.Collections.Generic;
using System.Linq;

namespace Code
{
    // TODO:
    // - use an immutable dictionary
    // - try to avoid use of ToDictionary inside ReduceDictionary
    // - try to eliminate the Tuple e.g. using a recursive loop passing totalSoFar or new dictionary
    //  - keep recursively calling CalculatePriceOfBooks until it is empty ?

    public class PotterCalculator
    {
        public static decimal Calculate(params Book[] books)
        {
            return CalculatePriceOfBooks(MakeDictionary(books));
        }

        private static IDictionary<string, int> MakeDictionary(IEnumerable<Book> books)
        {
            return books
                .GroupBy(book => book.Name)
                .ToDictionary(grouping => grouping.Key, grouping => grouping.Count());
        }

        private static decimal CalculatePriceOfBooks(IDictionary<string, int> dictionary)
        {
            if (dictionary.Count == 0) return 0m;
            if (dictionary.Count == 1) return dictionary.First().Value * 8m;
            return ReduceDictionary(dictionary);
        }

        private static decimal ReduceDictionary(IDictionary<string, int> dictionary)
        {
            var differentBooks = dictionary.Keys;

            IDictionary<string, int> newDictionary = dictionary
                .Select(kvp => new KeyValuePair<string, int>(kvp.Key, kvp.Value - 1))
                .Where(kvp => kvp.Value > 0)
                .GroupBy(kvp => kvp.Key)
                .ToDictionary(grouping => grouping.Key, grouping => grouping.Count());

            return DiscountedPriceOfBooks(differentBooks) + CalculatePriceOfBooks(newDictionary);
        }

        private static decimal DiscountedPriceOfBooks(IEnumerable<string> differentBooks)
        {
            var numDifferentBooks = differentBooks.Count();
            var subTotal = numDifferentBooks * 8m;

            switch (numDifferentBooks)
            {
                case 2:
                    return subTotal * 0.95m;

                case 3:
                    return subTotal * 0.90m;

                case 4:
                    return subTotal * 0.80m;

                case 5:
                    return subTotal * 0.75m;
 
                default:
                    throw new InvalidOperationException(string.Format("Expected between 2 and 5 books but got {0}.", numDifferentBooks));
            }
        }
    }
}
