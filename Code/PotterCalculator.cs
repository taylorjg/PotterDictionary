using System;
using System.Collections.Generic;
using System.Linq;

namespace Code
{
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
            var total = 0m;

            for (; ; )
            {
                switch (dictionary.Count)
                {
                    case 0:
                        return total;

                    case 1:
                        return total + dictionary.First().Value * 8m;

                    default:
                        var tuple = ReduceDictionary(dictionary);
                        dictionary = tuple.Item1;
                        total += tuple.Item2;
                        break;
                }
            }
        }

        private static Tuple<IDictionary<string, int>, decimal> ReduceDictionary(IDictionary<string, int> dictionary)
        {
            var differentBooks = dictionary.Keys;

            IDictionary<string, int> newDictionary = dictionary
                .Select(kvp => new KeyValuePair<string, int>(kvp.Key, kvp.Value - 1))
                .Where(kvp => kvp.Value > 0)
                .GroupBy(kvp => kvp.Key)
                .ToDictionary(grouping => grouping.Key, grouping => grouping.Count());

            return Tuple.Create(newDictionary, DiscountedPriceOfBooks(differentBooks));
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
