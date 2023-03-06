using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PFCWebApp.Models
{
    [FirestoreData]
    public class Book
    {
        [FirestoreProperty]
        [Required]
        public string Isbn { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public int Year { get; set; }
        [FirestoreProperty]
        public string Category { get; set; }

        [FirestoreProperty]
        public string Link { get; set; }
    }
}
