using Microsoft.AspNetCore.Mvc;
using PFCWebApp.DataAccess;
using PFCWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;

namespace PFCWebApp.Controllers
{
    public class ReservationsController : Controller
    {
        FirestoreBooksRepository _fbr;
        FirestoreReservationsRepository _frr;
        PubSubEmailRepository _pser;
        public ReservationsController(FirestoreBooksRepository fbr, FirestoreReservationsRepository frr
            , PubSubEmailRepository pser)
        {
            _fbr = fbr;
            _frr = frr;
            _pser = pser;
        }

        public async Task<IActionResult> Index()
        {
            var listOfReservations = await _frr.GetReservations();
            return View(listOfReservations);
        }

        public async Task<IActionResult> Create(string isbn )
        {
            var book = await _fbr.GetBook(isbn);
            ViewBag.Isbn = book.Isbn;
            return View();
        
        }
    
        [HttpPost]
        public async Task<IActionResult> Create(Reservation r, System.DateTime from,DateTime to)
        {
            //converting from System.Datetime to Google.Cloud.Firestore.Timestamp
            try
            {
                r.From = Google.Cloud.Firestore.Timestamp.FromDateTime(from.ToUniversalTime());
                r.To = Google.Cloud.Firestore.Timestamp.FromDateTime(to.ToUniversalTime());

                //adds the reservation to the nosql database
                await _frr.AddReservation(r);

                //pushes the reservation onto a queue so eventually user will be notified by email as a confirmation
                await _pser.PushMessage(r);

                var docId = _fbr.GetBookId(r.Isbn);

                //await _pser.PushMessage(docId)


                TempData["success"] = "Reservation added";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Reservation creation failed";
            }

            return View();
        
        }

        public async Task<IActionResult> History(string isbn)
        {
            var list = await _frr.GetReservations(isbn);
            return View("Index", list);
        }
    }
}
