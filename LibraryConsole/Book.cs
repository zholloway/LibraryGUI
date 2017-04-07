using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryConsole
{
    class Book
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int YearPublished { get; set; }
        public string Genre { get; set; }
        public bool IsCheckedOut { get; set; }
        public DateTime LastCheckedOutDate { get; set; }
        public DateTime DueBackDate { get; set; }

        public Book() { }
    }
}
