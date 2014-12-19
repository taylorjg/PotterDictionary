using System;
using System.Linq;
using Code;

namespace App
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var booksString = args.Length == 1 ? args[0] : "ABC";
            var books = booksString.Select(c => new Book(new string(c, 1))).ToArray();
            var total = PotterCalculator.Calculate(books);
            Console.WriteLine("Total for \"{0}\": {1}.", booksString, total);
        }
    }
}
