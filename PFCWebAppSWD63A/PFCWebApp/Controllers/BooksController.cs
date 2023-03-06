using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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

        public async Task<IActionResult> Index()
        {
           var list = await  _booksRepo.GetBooks();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        { return View(); }

        [HttpPost]
        public IActionResult Create(Book b, IFormFile file, [FromServices] IConfiguration config ) //an example of Method Injection
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string bucketName = config["bucket"].ToString();
                    if (file != null)
                    {
                        //1. Upload the e-book (file) in the cloud bucket
                        var storage = StorageClient.Create();
                        using var fileStream = file.OpenReadStream();

                        string newFilename = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);

                        storage.UploadObject(bucketName, newFilename, null, fileStream);

                        b.Link = $"https://storage.googleapis.com/{bucketName}/{newFilename}";
                    }

                    //2. save the link together with the rest of the textual data into the NoSql db

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


        public async Task<IActionResult> Delete(string isbn)
        {
            try
            {
                await _booksRepo.Delete(isbn);
                TempData["success"] = "Book was deleted successfully from the database";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Book could not be deleted";

                //logging ex in google cloud (Error reporting)
            }
            return RedirectToAction("Index");
        }


        [HttpGet] //when the user clicks on the Edit link, this is going to run and
                  //its going to load the original data from the database and show it to the user
                  //PRE EDITING
        public async Task<IActionResult> Edit(string isbn)
        {
            var book  = await _booksRepo.GetBook(isbn);
            return View(book);
        }

        [HttpPost] //Once the user writes the data and clicks the submit button, the edited data
                    // is going to be sent to this method to be saved in our NOSQL db
        public IActionResult Edit(Book b)
        {
            try
            {
                _booksRepo.Update(b);
                TempData["success"] = "Book was updated";
            }
            catch (Exception ex)

            {
                TempData["error"] = "Book could not be updated";
            }
            return View(b);
        }

    }
}
