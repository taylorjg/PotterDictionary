using System;
using System.Collections.Generic;
using System.Linq;
using Code;

namespace App
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var commaSeparatedTitles = (args.Length == 1) ? args[0] : "A,A,B";
            var titles = SplitCommaSeparatedTitles(commaSeparatedTitles).ToArray();
            var total = PotterCalculator.CalculatePrice(titles);
            Console.WriteLine("Total for {{{0}}}: {1}.", string.Join(",", titles), total);
        }

        private static IEnumerable<string> SplitCommaSeparatedTitles(string commaSeparatedTitles)
        {
            return commaSeparatedTitles
                .Split(',')
                .Select(title => title.Trim())
                .Where(title => title.Length > 0);
        }
    }
}
