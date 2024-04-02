using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureStorageActions.FunctionApp.Interfaces;
using AzureStorageActions.FunctionApp.Models;
using AzureStorageActions.FunctionApp.Utilities;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureStorageActions.FunctionApp.Services
{
    /// <summary>
    /// Service for tagging and recovering blobs in Azure Blob Storage.
    /// </summary>
    public class BlobStorageTaggerService : IBlobTagger
    {
        /// <summary>
        /// Configuration settings for the BlobStorageTaggerService.
        /// </summary>
        public class Configuration
        {
            const string ConfigRootName = "BlobStorageTaggerService";

            /// <summary>
            /// Gets or sets the list of container names to tag.
            /// </summary>
            public IEnumerable<string> Containers { get; set; }

            /// <summary>
            /// Gets or sets the prefix for the blob names to tag.
            /// </summary>
            public string BlobPrefix { get; set; }

            /// <summary>
            /// Gets or sets the key-value pair for the tag to apply.
            /// </summary>
            public KeyValuePair<string, string> Tag { get; set; }

            /// <summary>
            /// Gets or sets the connection string for Azure Blob Storage.
            /// </summary>
            public string StorageAccessKey { get; set; }

            /// <summary>
            /// Gets a value indicating whether to use managed identity for authentication.
            /// </summary>
            public bool UseManagedIdentity { get => string.IsNullOrEmpty(this.StorageAccessKey); }

            /// <summary>
            /// Loads the configuration settings from the provided IConfiguration instance.
            /// </summary>
            /// <param name="config">The IConfiguration instance.</param>
            /// <returns>An instance of Configuration with the loaded settings.</returns>
            public static Configuration Load(IConfiguration config)
            {
                var retVal = new Configuration();

                var containers = config[$"{ConfigRootName}:Containers"];
                retVal.Containers = containers.Split(new char[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);

                retVal.BlobPrefix = config[$"{ConfigRootName}:BlobPrefix"];
                retVal.Tag = new KeyValuePair<string, string>(
                    config[$"{ConfigRootName}:TagKey"],
                    config[$"{ConfigRootName}:TagValue"]
                );

                retVal.StorageAccessKey = config[$"{ConfigRootName}:AccessKey"];
                return retVal;
            }


        }

        private readonly ILogger<BlobStorageTaggerService> logger;
        private readonly Configuration configuration;

        /// <summary>
        /// Initializes a new instance of the BlobStorageTaggerService class.
        /// </summary>
        /// <param name="logger">The ILogger instance.</param>
        /// <param name="configuration">The IConfiguration instance.</param>
        public BlobStorageTaggerService(ILogger<BlobStorageTaggerService> logger,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = Configuration.Load(configuration);
        }

        public async Task<bool> TagAsync(BlobCreatedData blobData, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(blobData);

            if (!UrlUtility.IsContainerNamesMatch(blobData.url, configuration.Containers))
            {
                logger.LogInformation($"Blob {blobData.url} is not in a supported container.");
                return false;
            }
            if (!UrlUtility.IsBlobNameStartsWith(blobData.url, configuration.BlobPrefix))
            {
                logger.LogInformation($"Blob {blobData.url} does not have the expected prefix.");
                return false;
            }

            BlobClient blobClient = null;

            if (this.configuration.UseManagedIdentity)
            {
                blobClient = new BlobClient(new Uri(blobData.url),
                    new ManagedIdentityCredential());
            }
            else
            {
                var storageName = UrlUtility.ExtractStorageName(blobData.url); 
                blobClient = new BlobClient(new Uri(blobData.url),
                    new StorageSharedKeyCredential(storageName,this.configuration.StorageAccessKey));
            }

            try
            {
                await blobClient.SetTagsAsync(new Dictionary<string, string>
                    {
                        { this.configuration.Tag.Key, this.configuration.Tag.Value }
                    },
                    null,  
                    cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error tagging blob {blobData.url}");
                return false;
            }
        }
    }

}
