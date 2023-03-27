using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        ILogger<BooksController> _logger;
        public BooksController(FirestoreBooksRepository booksRepo, ILogger<BooksController> logger)
        {
            _logger = logger;
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

        [HttpPost][Authorize]
        public IActionResult Create(Book b, IFormFile file, [FromServices] IConfiguration config ) //an example of Method Injection
        {
            _logger.LogInformation($"User {User.Identity.Name} is creating a book with {b.Isbn}");
            if (ModelState.IsValid)
            {

                _logger.LogInformation($"Validators for {b.Isbn} are ok");
                try
                {
                    string bucketName = config["bucket"].ToString();
                    if (file != null)
                    {
                        //1. Upload the e-book (file) in the cloud bucket
                        var storage = StorageClient.Create();
                        using var fileStream = file.OpenReadStream();

                        _logger.LogInformation($"File {file.FileName} is about to be uploaded");

                        string newFilename = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                        _logger.LogInformation($"File {file.FileName} has been renamed to {newFilename}");

                        storage.UploadObject(bucketName, newFilename, null, fileStream);
                        _logger.LogInformation($"File {file.FileName} with new filename {newFilename} has been uploaded successfully");

                        b.Link = $"https://storage.googleapis.com/{bucketName}/{newFilename}";
                        _logger.LogInformation($"File {file.FileName} with new filename {newFilename} can be found here {b.Link}");

                    }

                    //2. save the link together with the rest of the textual data into the NoSql db
                    _logger.LogInformation($"Book with {b.Isbn} will be save to db");

                    _booksRepo.AddBook(b);

                    _logger.LogInformation($"Book with {b.Isbn} was saved to db");

                    TempData["success"] = "Book was added successfully in database";
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"{User.Identity.Name} had an error while uploading a file");
                    TempData["error"] = "Book was not added in the database";
                }
            }
            else
            {
                string jsonWarnings = JsonConvert.SerializeObject(ModelState.Values);
                _logger.LogWarning($"Validation errors: {jsonWarnings}");
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
