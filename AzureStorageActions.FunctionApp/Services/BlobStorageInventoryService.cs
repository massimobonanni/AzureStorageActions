using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using AzureStorageActions.FunctionApp.Interfaces;
using AzureStorageActions.FunctionApp.Models;
using AzureStorageActions.FunctionApp.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using System.Text.Json;

namespace AzureStorageActions.FunctionApp.Services
{
    public class BlobStorageInventoryService : IBlobInventoryAnalyzer
    {

        public class Configuration
        {
            const string ConfigRootName = "BlobStorageInventoryService";

            public KeyValuePair<string, string> Tag { get; set; }

            public string StorageAccessKey { get; set; }

            public bool UseManagedIdentity { get => string.IsNullOrEmpty(this.StorageAccessKey); }

            public static Configuration Load(IConfiguration config)
            {
                var retVal = new Configuration();

                retVal.Tag = new KeyValuePair<string, string>(
                    config[$"{ConfigRootName}:TagKey"],
                    config[$"{ConfigRootName}:TagValue"]
                );

                retVal.StorageAccessKey = config[$"{ConfigRootName}:AccessKey"];
                return retVal;
            }


        }

        private readonly ILogger<BlobStorageInventoryService> logger;
        private readonly Configuration configuration;

        public BlobStorageInventoryService(ILogger<BlobStorageInventoryService> logger,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = Configuration.Load(configuration);
        }

        public async Task<bool> AnalyzeAsync(BlobInventoryPolicyCompletedData inventoryData, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(inventoryData);

            var result = true;
            var inventoryManifest = await ReadInventoryManifestFile(inventoryData.manifestBlobUrl);
            if (inventoryManifest != null)
            {
                var blobsToDelete = await this.AnalyzeAsync(inventoryManifest);
                if (blobsToDelete != null && blobsToDelete.Any())
                {
                    foreach (var blob in blobsToDelete)
                    {
                        var blobClient = CreateBlobClient(blob);
                        try
                        {
                            await blobClient.DeleteAsync();
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, $"Error deleting blob {blob}");
                        }
                    }
                }
            }
            return result;
        }

        private BlobClient CreateBlobClient(string blobUri)
        {
            BlobClient blobClient = null;

            if (this.configuration.UseManagedIdentity)
            {
                blobClient = new BlobClient(new Uri(blobUri),
                    new ManagedIdentityCredential());
            }
            else
            {
                var storageName = UrlUtility.ExtractStorageName(blobUri);
                blobClient = new BlobClient(new Uri(blobUri),
                    new StorageSharedKeyCredential(storageName, this.configuration.StorageAccessKey));
            }

            return blobClient;
        }

        private async Task<InventoryManifest> ReadInventoryManifestFile(string manifestBlobUrl)
        {
            InventoryManifest? result = null;
            try
            {
                var blobClient = CreateBlobClient(manifestBlobUrl);
                var blobContent = await blobClient.DownloadContentAsync();
                result = blobContent?.Value?.Content?.ToObjectFromJson<InventoryManifest>(new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                result = null;
            }

            return result;
        }

        private const string TagsColumnHeader = "Tags";
        private const string NameColumnHeader = "Name";

        public async Task<IEnumerable<string>> AnalyzeAsync(InventoryManifest manifest)
        {
            var blobsToDelete = new List<string>();
            if (manifest.Files.Any())
            {
                try
                {
                    foreach (var file in manifest.Files)
                    {
                        var blobFullName = $"{manifest.Endpoint}/{manifest.DestinationContainer}/{file.Blob}";
                        var blobClient = CreateBlobClient(blobFullName);
                        var blobContent = await blobClient.DownloadContentAsync();

                        using (TextFieldParser parser = new TextFieldParser(blobContent.Value.Content.ToStream()))
                        {
                            parser.TextFieldType = FieldType.Delimited;
                            parser.SetDelimiters(",");
                            var rowIndex = 0;
                            int tagsColumnIndex = 0, nameColumnIndex = 0;
                            while (!parser.EndOfData)
                            {
                                string[] fields = parser.ReadFields();
                                if (rowIndex == 0)
                                {
                                    tagsColumnIndex = fields.Select((elem, index) => new { elem, index })
                                            .First(p => p.elem == TagsColumnHeader)
                                            .index;
                                    nameColumnIndex = fields.Select((elem, index) => new { elem, index })
                                            .First(p => p.elem == NameColumnHeader)
                                            .index;
                                }
                                else
                                {
                                    if (fields.ContainsTag(tagsColumnIndex, this.configuration.Tag))
                                    {
                                        blobsToDelete.Add($"{manifest.Endpoint}/{fields[nameColumnIndex]}");
                                    }
                                }
                            }
                            rowIndex++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, $"Error analyzing manifest {manifest.RuleName}");
                    blobsToDelete = null;
                }
            }
            return blobsToDelete;
        }
    }

}
