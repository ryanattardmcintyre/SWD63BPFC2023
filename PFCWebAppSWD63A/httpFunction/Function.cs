using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace httpFunction;

public class Function : IHttpFunction
{
    /// <summary>
    /// Logic for your function goes here.
    /// </summary>
    /// <param name="context">The HTTP context, containing the request and the response.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task HandleAsync(HttpContext context)
    { 

           //    string credential_path =  "swd63b2023-08d5b155ddab.json";
         //   System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);

        HttpRequest request = context.Request;
            // Check URL parameters for "message" field
            string message = request.Query["message"];

        Book b = await GetBook(message);


        await context.Response.
        WriteAsync($"<div>Name {b.Name}, Author {b.Year}, <a href=\"{b.Link}\">Download</a></div>");
    }

  private async Task<Book> GetBook(string isbn)
        {
           var db = FirestoreDb.Create("swd63b2023");
            Query booksQuery = db.Collection("books").WhereEqualTo("Isbn", isbn);
            QuerySnapshot booksQuerySnapshot = await booksQuery.GetSnapshotAsync();
           
            DocumentSnapshot documentSnapshot = booksQuerySnapshot.Documents.FirstOrDefault();
            if (documentSnapshot.Exists == false) return null;
            else
            {
                Book result = documentSnapshot.ConvertTo<Book>();
                return result;
            }
        }


}
