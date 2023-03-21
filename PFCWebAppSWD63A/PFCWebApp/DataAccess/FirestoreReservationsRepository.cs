using Google.Cloud.Firestore;
using PFCWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
namespace PFCWebApp.DataAccess
{
    public class FirestoreReservationsRepository
    {
        FirestoreDb db;
        ///books/0l7DCJjawwlaHEqghoZR/reservations/qqbkAMjpPHeiYbi2qRI1
        ///

        FirestoreBooksRepository _fbr;
        public FirestoreReservationsRepository(string project, FirestoreBooksRepository fbr)
        { 
            db = FirestoreDb.Create(project);
            _fbr = fbr;
        }

        public async Task AddReservation(Reservation r)
        {
            var id = await _fbr.GetBookId(r.Isbn);
            await db.Collection($"books/{id}/reservations").Document().SetAsync(r);
            
        }

        public async Task<List<Reservation>> GetReservations()
        {
            List<Reservation> reservations = new List<Reservation>();

            var listOfBooks = await _fbr.GetBooks();
            foreach (var b in listOfBooks)
            {
                var docIdForBook = await _fbr.GetBookId(b.Isbn);

               var listOfReservations = 
                    await db.Collection($"books/{docIdForBook}/reservations").GetSnapshotAsync();

                foreach (DocumentSnapshot documentSnapshot in listOfReservations.Documents)
                {
                    Reservation r = documentSnapshot.ConvertTo<Reservation>();
                    reservations.Add(r);
                }
            }
            return reservations;
        }

        public async Task<List<Reservation>> GetReservations(string isbn)
        {
            List<Reservation> reservations = new List<Reservation>();

            var docIdForBook = await _fbr.GetBookId(isbn);

            var listOfReservations =
                 await db.Collection($"books/{docIdForBook}/reservations").GetSnapshotAsync();

            if (listOfReservations.Count == 0) return reservations;

            foreach (DocumentSnapshot documentSnapshot in listOfReservations.Documents)
            {
                Reservation r = documentSnapshot.ConvertTo<Reservation>();
                reservations.Add(r);
            }

            return reservations;
        }
    }
}
