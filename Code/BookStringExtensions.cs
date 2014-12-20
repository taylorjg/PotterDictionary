using System.Collections.Generic;
using System.Linq;

namespace Code
{
    public static class BookStringExtensions
    {
        public static IEnumerable<Book> ToBooks(this string bookNames)
        {
            return bookNames
                .Split(',')
                .Select(name => name.Trim())
                .Where(name => name.Length > 0)
                .Select(name => new Book(name));
        }
    }
}
