using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using System.Threading;
using Common.Models;
using Newtonsoft.Json;
using System.Net.Http;
using RestSharp;
using RestSharp.Authenticators;
using Mailgun.Service;
using Mailgun.Messages;

namespace SubscriberApp.Controllers
{
    public class SubscriberController : Controller
    {
        public async Task<IActionResult> Index()
        {
            string projectId = "swd63b2023";
            string subscriptionId = "messages-sub";
            bool acknowledge = true;

            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
            SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);
            // SubscriberClient runs your message handle function on multiple
            // threads to maximize throughput.
            int messageCount = 0;

            List<string> messages = new List<string>();


            Task startTask = subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
            {
                string text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());
                messages.Add($"{message.MessageId}: {text}");
               
                Interlocked.Increment(ref messageCount);
                //if(acknowledge == true) { return SubscriberClient.Reply.Ack} else {return SubscriberClient.Reply.Nack}

                return Task.FromResult(acknowledge ? SubscriberClient.Reply.Ack : SubscriberClient.Reply.Nack);

                //acknowledgement implies that the message is going to be removed from the queue
                //no acknowledgement implies that the message is not going to be removed from the queue
            });
            // Run for 5 seconds.
            await Task.Delay(5000);
            await subscriber.StopAsync(CancellationToken.None);
            // Lets make sure that the start task finished successfully after the call to stop.
            await startTask;
            

            //evaluate the messages list

            foreach(var msg in messages.Distinct().ToList())
            {
                //send emails with the details
                var actualMessage = msg.Split(": ")[1];
                Reservation myReadReservation = JsonConvert.DeserializeObject<Reservation>(actualMessage);

                //email sending code
 
                var mg = new MessageService(""); //apikey
                //var mg = new MessageService(ApiKey,false); //you can specify to use SSL or not, which determines the url API scheme to use
                //var mg = new MessageService(ApiKey,false,"api.mailgun.net/v3"); //you can also override the base URL, which defaults to v2

                //build a message
                var message = new MessageBuilder()
                     .AddToRecipient(new Recipient
                     {
                         Email = myReadReservation.Email,
                         DisplayName = "Charlie King"
                     })
                     .SetTestMode(true)
                     .SetSubject("Reservation confirmed")
                     .SetFromAddress(new Recipient { Email = "bringking@gmail.com", DisplayName = "Mailgun C#" })
                     .SetTextBody($"Reservation was made from {myReadReservation.DtFrom.ToLongDateString()} till {myReadReservation.DtTo.ToLongDateString()}. Enjoy")
                     .GetMessage();

                var content = await mg.SendMessageAsync("", message);
              //  content.ShouldNotBeNull();
            }

            return Content("Messages read and processed from queue: " + messageCount.ToString());
        }
    }
}
