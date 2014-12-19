using System;
using System.Collections.Generic;
using System.Linq;

namespace Code
{
    public class PotterCalculator
    {
        public static decimal Calculate(params Book[] books)
        {
            var total = 0m;
            var dictionary = MakeDictionary(books);

            for (; ; )
            {
                switch (dictionary.Count)
                {
                    case 0:
                        return total;

                    case 1:
                        return total + dictionary.First().Value * 8m;

                    default:
                        total += ReduceDictionary(dictionary);
                        break;
                }
            }
        }

        private static IDictionary<string, int> MakeDictionary(IEnumerable<Book> books)
        {
            return books
                .GroupBy(book => book.Name)
                .ToDictionary(grouping => grouping.Key, grouping => grouping.Count());
        }


        private static decimal ReduceDictionary(IDictionary<string, int> dictionary)
        {
            var setOfBooks = dictionary.Keys.ToList();
            foreach (var key in setOfBooks) dictionary[key] = dictionary[key] - 1;

            var keysWithZeroCount = dictionary.Where(kvp => kvp.Value == 0).Select(kvp => kvp.Key).ToList();
            foreach (var key in keysWithZeroCount) dictionary.Remove(key);

            return DiscountedPriceOfBooks(setOfBooks);
        }

        private static decimal DiscountedPriceOfBooks(IEnumerable<string> setOfBooks)
        {
            var bookCount = setOfBooks.Count();
            var subTotal = 8m * bookCount;

            switch (bookCount)
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
                    throw new InvalidOperationException(string.Format("Expected between 2 and 5 books but got {0}.", bookCount));
            }
        }
    }
}
