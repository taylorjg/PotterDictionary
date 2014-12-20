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
}
