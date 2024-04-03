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
    public class BlobDeleter
    {
        private readonly ILogger<BlobDeleter> logger;
        private readonly IBlobInventoryAnalyzer inventoryAnalyzer;

        public BlobDeleter(ILogger<BlobDeleter> logger, IBlobInventoryAnalyzer inventoryAnalyzer)
        {
            this.logger = logger;
            this.inventoryAnalyzer = inventoryAnalyzer;
        }

        [Function("BlobDeleter")]
        public async Task Run([EventGridTrigger] EventGridEvent eventGridEvent)
        {
            logger.LogInformation(eventGridEvent.Data.ToString());

            if (eventGridEvent.EventType != StorageAccountEvents.BlobInventoryPolicyCompleted)
            {
                logger.LogWarning("Event type is not supported.");
                return;
            }

            var data = JsonSerializer.Deserialize<BlobInventoryPolicyCompletedData>(eventGridEvent.Data.ToString(),
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                }
            );

            var result = await this.inventoryAnalyzer.AnalyzeAsync(data);

            logger.LogInformation($"Inventory {data.ruleName} analyzed with result {result}.");
        }
    }
}
