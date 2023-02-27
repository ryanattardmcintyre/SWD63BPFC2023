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

        public async Task<string> GetBookId (string isbn)
        {
            Query booksQuery = db.Collection("books").WhereEqualTo("Isbn", isbn);
            QuerySnapshot booksQuerySnapshot = await booksQuery.GetSnapshotAsync();

            DocumentSnapshot documentSnapshot = booksQuerySnapshot.Documents.FirstOrDefault();
            if (documentSnapshot.Exists == false) throw new Exception("Book does not exist");
            else
            {
                var id = documentSnapshot.Id;
                return id;
            }
        }

        public async void Update(Book b)
        {
            Query booksQuery = db.Collection("books").WhereEqualTo("Isbn", b.Isbn);
            QuerySnapshot booksQuerySnapshot = await booksQuery.GetSnapshotAsync();

            DocumentSnapshot documentSnapshot = booksQuerySnapshot.Documents.FirstOrDefault();
            if (documentSnapshot.Exists == false) throw new Exception("Book does not exist");
            else
            {
                DocumentReference booksRef = db.Collection("books").Document(documentSnapshot.Id);
                await booksRef.SetAsync(b);
            }
        }

        public async Task<Book> GetBook(string isbn)
        {
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

        public async Task Delete(string isbn) {

            Query booksQuery = db.Collection("books").WhereEqualTo("Isbn", isbn);
            QuerySnapshot booksQuerySnapshot = await booksQuery.GetSnapshotAsync();

            DocumentSnapshot documentSnapshot = booksQuerySnapshot.Documents.FirstOrDefault();
            if (documentSnapshot.Exists == false) throw new Exception("Book does not exist");
            else
            {
                 DocumentReference booksRef = db.Collection("books").Document(documentSnapshot.Id);
                 await booksRef.DeleteAsync();
            }
        }
    }
}
