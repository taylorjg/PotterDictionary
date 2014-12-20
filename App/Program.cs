using System;
using System.Linq;
using Code;

namespace App
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var bookNames = (args.Length == 1) ? args[0] : "A,A,B";
            var books = bookNames.ToBooks().ToArray();
            var total = PotterCalculator.CalculatePrice(books);
            Console.WriteLine("Total for {{{0}}}: {1}.", string.Join(",", books.Select(b => b.Name)), total);
        }
    }
}
