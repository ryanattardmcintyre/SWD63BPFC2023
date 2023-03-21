using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Common.Models
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

        public DateTime DtTo
        {
            get
            {
                return To.ToDateTime();
            }
            set
            {
              To =  Google.Cloud.Firestore.Timestamp.FromDateTime(value.ToUniversalTime());
            }
        }

        public DateTime DtFrom
        {
            get { return From.ToDateTime(); }
            set { From = Google.Cloud.Firestore.Timestamp.FromDateTime(value.ToUniversalTime()); }
        }
    }
}
