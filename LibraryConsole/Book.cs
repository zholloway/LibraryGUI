using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryConsole
{
    class Book
    {

        public string ID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string YearPublished { get; set; }
        public string Genre { get; set; }
        public string IsCheckedOut { get; set; }
        public string LastCheckedOutDate { get; set; }
        public string DueBackDate { get; set; }

        public Book() { }

        public void SetPropertyValue(string bookAttribute, string newValue)
        {
            if (this.GetType().GetProperty(bookAttribute) != null)
            {
                var property = this.GetType().GetProperty(bookAttribute);
                property.SetValue(this, newValue);
            }
        }
    }
}
