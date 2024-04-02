// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using Azure.Storage.Blobs;
using AzureStorageActions.FunctionApp.Models;
using AzureStorageActions.FunctionApp.Interfaces;

namespace AzureStorageActions.FunctionApp
{
    public class BlobTagger
    {
        private readonly ILogger<BlobTagger> logger;
        private readonly IBlobTagger blobTagger;

        public BlobTagger(ILogger<BlobTagger> logger, IBlobTagger blobTagger)
        {
            this.logger = logger;
            this.blobTagger = blobTagger;
        }

        [Function("BlobTagger")]
        public async Task Run([EventGridTrigger] EventGridEvent eventGridEvent)
        {
            logger.LogInformation(eventGridEvent.Data.ToString());

            if (eventGridEvent.EventType != "Microsoft.Storage.BlobCreated")
            {
                logger.LogWarning("Event type is not supported.");
                return;
            }

            var data = JsonSerializer.Deserialize<BlobCreatedData>(eventGridEvent.Data.ToString(),
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                }
            );

            var result = await this.blobTagger.TagAsync(data);

            logger.LogInformation($"Blob {data.url} tagged with result {result}.");
        }
    }
}
