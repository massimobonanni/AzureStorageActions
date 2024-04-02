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
    public class BlobRecover
    {
        private readonly ILogger<BlobRecover> logger;
        private readonly IBlobRecoverer blobRecoverer;

        public BlobRecover(ILogger<BlobRecover> logger, IBlobRecoverer blobRecoverer)
        {
            this.logger = logger;
            this.blobRecoverer = blobRecoverer;
        }

        [Function("BlobRecover")]
        public async Task Run([EventGridTrigger] EventGridEvent eventGridEvent)
        {
            logger.LogInformation(eventGridEvent.Data.ToString());
            var data = JsonSerializer.Deserialize<BlobDeletedData>(eventGridEvent.Data.ToString(),
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                }
            );

            var result = await this.blobRecoverer.RecoverAsync(data);

            if (result)
                logger.LogInformation($"Blob {data.url} recovered successfully.");
            else
                logger.LogWarning($"Blob {data.url} recovery failed.");

        }
    }
}
