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
        const string PathToDatabase = @"Server=localhost\SQLEXPRESS;Database=Library;Trusted_Connection=True;";

        static void Main(string[] args)
        {
            //present GUI options
            Console.WriteLine("Welcome to the library GUI. Your options are as follows:");
            Console.WriteLine("See information about all of the books               [1]");
            Console.WriteLine("Add a new book to the library                        [2]");
            Console.WriteLine("Delete a book                                        [3]");
            Console.WriteLine("Update a book's information                          [4]");
            Console.WriteLine("See what books are checked out/are available         [5]");
            /*not done*/Console.WriteLine("Check out a book                                     [6]");
            

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
                    YearPublished = year
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
            //delete a book
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
            //update book
            else if (int.Parse(userChoice) == 4)
            {
                Console.WriteLine("Enter the ID of the book you want to udpate");
                var bookID = Console.ReadLine();

                using (var connection = new SqlConnection(PathToDatabase))
                {
                    var cmd = new SqlCommand("SELECT * FROM Book WHERE ID = @ID", connection);
                    cmd.Parameters.AddWithValue("@ID", bookID);
                    connection.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine("Here is all of the information for the book you want to update:");
                        Console.WriteLine($"Title = {reader["Title"]}");
                        Console.WriteLine($"Author = {reader["Author"]}");
                        Console.WriteLine($"YearPublished = {reader["YearPublished"]}");
                        Console.WriteLine($"Genre = {reader["Genre"]}");
                        Console.WriteLine($"IsCheckedOut = {reader["IsCheckedOut"]}");
                        Console.WriteLine($"Last Checkout Date = {reader["LastCheckedOutDate"]}");
                        Console.WriteLine($"Due Back Date = {reader["DueBackDate"]}");
                    }
                    connection.Close();
                }

                Console.WriteLine("Enter the attribute you want to change (Title, Author, etc)");
                var bookAttribute = Console.ReadLine();
                Console.WriteLine("Enter the value you want to update the attribute to");
                var bookValue = Console.ReadLine();
                
                var bookToUpdate = new BookToUpdate
                {
                    ID = bookID,
                    attribute = bookAttribute,
                    newValue = bookValue
                };
                
                string json = JsonConvert.SerializeObject(bookToUpdate);
                using (var writer = new StreamWriter(PathToJson))
                {
                    writer.WriteLine(json);
                }
                
                //use json for Put request
                url = $"http://localhost:52489/api/library/UpdateBook?ID={bookID}&attribute={bookAttribute}&newValue={bookValue}";
                var request = WebRequest.Create(url);
                //set content type and method to application/json // PUT so API knows it can use it
                request.Method = "POST";
                //getRequestStream opens a stream to the API that you can write data to
                
                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(json);
                    writer.Flush();
                }
                
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
            //get checked out/available books
            else if (int.Parse(userChoice) == 5)
            {
                Console.WriteLine("Do you want to see [checked out] books or [available] books?");
                var checkedOutStatus = Console.ReadLine();
                var isCheckedOut = String.Empty;
                if (checkedOutStatus == "checked out")
                {
                    isCheckedOut = "True";
                }
                else if (checkedOutStatus == "available")
                {
                    isCheckedOut = "False";
                }


                url = $"http://localhost:52489/api/checkout/GetBooks?IsCheckedOut={isCheckedOut}";
                var request = WebRequest.Create(url);
                var response = request.GetResponse();
                var rawResponse = String.Empty;
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    while (reader.Peek() > -1)
                    {
                        rawResponse = reader.ReadLine();
                    }
                }

                var trimmedRawResponse = rawResponse.Trim(new Char[] { '[', ' ', ']' });
                var array = trimmedRawResponse.Split(',');
                foreach (var bookString in array)
                {
                    Console.WriteLine(bookString.Trim(new Char[] { '"' }));
                }
            }
            //check out a book
            else if (int.Parse(userChoice) == 6)
            {

            }

            Console.ReadLine();
        }
    }
}
