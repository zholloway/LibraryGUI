using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the library GUI. Your options are as follows:");
            Console.WriteLine("If you would like to see information about all of the books, please enter [1]");
            Console.WriteLine("If you would like to add a new book to the library, please enter [2]");
            var userChoice = Console.ReadLine();

            if(int.Parse(userChoice) == 1)
            {

            }
            else if (int.Parse(userChoice) == 2)
            {

            }
        }
    }
}
