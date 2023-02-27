using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PFCWebApp.Models
{
    [FirestoreData]
    public class Reservation
    {
        [FirestoreProperty]
        public string Isbn { get; set; }

        [FirestoreProperty]
        public string Email { get; set; }
        [FirestoreProperty]
        public Timestamp From { get; set; }
        
        [FirestoreProperty]
        public Timestamp To { get; set; }
    }
}
