using Microsoft.AspNetCore.Mvc;
using PFCWebApp.DataAccess;
using PFCWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PFCWebApp.Controllers
{
    public class ReservationsController : Controller
    {
        FirestoreBooksRepository _fbr;
        FirestoreReservationsRepository _frr;
        public ReservationsController(FirestoreBooksRepository fbr, FirestoreReservationsRepository frr)
        {
            _fbr = fbr;
            _frr = frr;
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

                await _frr.AddReservation(r);

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
