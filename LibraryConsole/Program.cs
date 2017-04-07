using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using System.Net;
using Newtonsoft.Json;

namespace LibraryConsole
{
    class Program
    {
        const string PathToJson = @"C:\Users\Zack Holloway\Documents\Visual Studio 2017\Projects\LibraryConsole\LibraryConsole\bin\Debug\Book.json";

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the library GUI. Your options are as follows:");
            Console.WriteLine("If you would like to see information about all of the books, please enter [1]");
            Console.WriteLine("If you would like to add a new book to the library, please enter [2]");
            var userChoice = Console.ReadLine();

            var url = String.Empty;

            if(int.Parse(userChoice) == 1)
            {
                url = "http://localhost:52489/api/library";
                var request = WebRequest.Create(url);
                var response = request.GetResponse();

                var rawResponse = String.Empty;
                
                using(var reader = new StreamReader(response.GetResponseStream()))
                {
                    while(reader.Peek() > -1)
                    {
                        rawResponse = reader.ReadLine();
                    }
                }

                var trimmedRawResponse = rawResponse.Trim(new Char[] { '[', ' ', ']'});
                var array = trimmedRawResponse.Split(',');
                foreach (var bookString in array)
                {
                    Console.WriteLine(bookString.Trim(new Char[] { '"' }));
                }
            }
            else if (int.Parse(userChoice) == 2)
            {
                Console.WriteLine("Please enter a book title:");
                var bookTitle = Console.ReadLine();
                Console.WriteLine("Please enter the year published:");
                var year = Console.ReadLine();

                var newBook = new Book
                {
                    Title = bookTitle,
                    YearPublished = int.Parse(year)
                };

                /*
                var dataForJson = $"Title = {newBook.Title}, YearPublished = {newBook.YearPublished}";
                string json = JsonConvert.SerializeObject(dataForJson.ToArray());
                File.WriteAllText(PathToJson, json);
                */
                
                url = $"http://localhost:52489/api/library/AddNewBook?Title={newBook.Title}&YearPublished={newBook.YearPublished}";
                var request = WebRequest.Create(url);
                var response = request.GetResponse();

                var rawResponse = String.Empty;

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    while (reader.Peek() > -1)
                    {
                        Console.WriteLine(reader.ReadLine());
                    }
                }
                
            }

            Console.ReadLine();
        }
    }
}
