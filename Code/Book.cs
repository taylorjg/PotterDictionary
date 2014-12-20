using System.Collections.Generic;
using System.Linq;

namespace Code
{
    public class Book
    {
        public Book(string name)
        {
            _name = name;
        }

        private readonly string _name;

        public string Name
        {
            get { return _name; }
        }
    }

    public static class BookStringExtensions
    {
        public static IEnumerable<Book> ToBooks(this string bookNames)
        {
            return bookNames
                .Split(',')
                .Select(name => name.Trim())
                .Select(name => new Book(name));
        }
    }
}
