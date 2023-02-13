using Google.Cloud.Firestore;
using PFCWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PFCWebApp.DataAccess
{
    public class FirestoreBooksRepository
    {
        FirestoreDb db;
        public FirestoreBooksRepository(string project)
        {
            db = FirestoreDb.Create(project);
        }

        public async void AddBook(Book b)
        {
            await db.Collection("books").Document().SetAsync(b);
        }

        public async Task<List<Book>> GetBooks()
        {
            List<Book> books = new List<Book>();
            Query allBooksQuery = db.Collection("books");
            QuerySnapshot allBooksQuerySnapshot = await allBooksQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in allBooksQuerySnapshot.Documents)
            {
                Book b = documentSnapshot.ConvertTo<Book>();
                books.Add(b);
            }

            return books;
        }
    }
}
