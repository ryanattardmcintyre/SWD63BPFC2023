using Microsoft.AspNetCore.Mvc;
using PFCWebApp.DataAccess;
using PFCWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PFCWebApp.Controllers
{
    public class BooksController : Controller
    {
        FirestoreBooksRepository _booksRepo;
        public BooksController(FirestoreBooksRepository booksRepo)
        {
            _booksRepo = booksRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        { return View(); }

        [HttpPost]
        public IActionResult Create(Book b )
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _booksRepo.AddBook(b);
                    TempData["success"] = "Book was added successfully in database";
                }
                catch (Exception e)
                {
                    TempData["error"] = "Book was not added in the database";
                }
            }
            
            return View();
        }
    }
}
