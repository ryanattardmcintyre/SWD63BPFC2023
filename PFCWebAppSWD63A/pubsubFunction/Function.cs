using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Events.Protobuf.Cloud.PubSub.V1;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace HelloPubSub
{
    public class Function : ICloudEventFunction<MessagePublishedData>
    {
        private readonly ILogger<Function> _logger;

        public Function(ILogger<Function> logger) => _logger = logger; 

        public Task HandleAsync(CloudEvent cloudEvent, MessagePublishedData data, CancellationToken cancellationToken)
        {
            _logger.LogInformation("PubSub function has started executing");
            var nameFromMessage = data.Message?.TextData;
            _logger.LogInformation($"Data received is {nameFromMessage}");

            var name = string.IsNullOrEmpty(nameFromMessage) ? "world" : nameFromMessage;
            
            _logger.LogInformation($"Name is {name}");

            FirestoreDb db = FirestoreDb.Create("swd63b2023");
            DocumentReference docRef = db.Collection("books").Document(nameFromMessage);
            Dictionary<string, object> update = new Dictionary<string, object>
            {
                { "status", "not available" }
            };
            var t = docRef.SetAsync(update, SetOptions.MergeAll);
            //code other things so that they are executed meanwhile

            t.Wait();
            return Task.CompletedTask;
        }
    }
}