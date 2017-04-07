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
            //present GUI options
            Console.WriteLine("Welcome to the library GUI. Your options are as follows:");
            Console.WriteLine("If you would like to see information about all of the books, please enter [1]");
            Console.WriteLine("If you would like to add a new book to the library, please enter [2]");
            Console.WriteLine("If you would like to delete a book, please enter [3]");
            /*not done*/Console.WriteLine("If you would like to update a book's information, please enter [4]");
            /*not done*/Console.WriteLine("If you would like to see what books are checked out/are available, please enter [5]");
            /*not done*/Console.WriteLine("If you would like to check out a book, please enter [6]");
            

            var userChoice = Console.ReadLine();

            var url = String.Empty;
            var ID = String.Empty;
            
            //Get all books
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
            //add new book to database
            else if (int.Parse(userChoice) == 2)
            {
                Console.WriteLine("Please enter a book title:");
                var bookTitle = Console.ReadLine();
                Console.WriteLine("Please enter the year published:");
                var year = Console.ReadLine();
                //use user input to create new Book object
                var newBook = new Book
                {
                    Title = bookTitle,
                    YearPublished = int.Parse(year)
                };
                //convert new Book to json and write to Book.json
                string json = JsonConvert.SerializeObject(newBook);
                using(var writer = new StreamWriter(PathToJson))
                {
                    writer.WriteLine(json);
                }
                //use json for Put request
                url = $"http://localhost:52489/api/library/AddNewBook";
                var request = WebRequest.Create(url);
                //set content type and method to application/json // PUT so API knows it can use it
                request.ContentType = "application/json";
                request.Method = "PUT";
                //getRequestStream opens a stream to the API that you can write data to
                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(json);
                    writer.Flush();                        
                }
                //now to get the response from the API after it uses the request json to perform its function
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
            else if (int.Parse(userChoice) == 3)
            {
                Console.WriteLine("Enter the ID of the book you would like to delete");
                ID = Console.ReadLine();

                url = $"http://localhost:52489/api/library?ID={ID}";
                var request = WebRequest.Create(url);
                request.Method = "DELETE";
                var response = request.GetResponse();
                

                var rawResponse = String.Empty;

                using(var reader = new StreamReader(response.GetResponseStream()))
                {
                    while(reader.Peek() > -1)
                    {
                        rawResponse = reader.ReadLine();
                    }
                }
                Console.WriteLine(rawResponse);
            }

            Console.ReadLine();
        }
    }
}
