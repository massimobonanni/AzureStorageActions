using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureStorageActions.FunctionApp.Interfaces;
using AzureStorageActions.FunctionApp.Models;
using AzureStorageActions.FunctionApp.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureStorageActions.FunctionApp.Services
{
    public class BlobStorageService : IBlobRecoverer
    {
        public class Configuration
        {
            const string ConfigRootName = "BlobStorageService";
            
            public IEnumerable<string> Containers { get; set; }
            public static Configuration Load(IConfiguration config)
            {
                var retVal = new Configuration();

                var containers = config[$"{ConfigRootName}:Containers"];
                retVal.Containers = containers.Split(new char[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
                return retVal;
            }
        }

        private readonly ILogger<BlobStorageService> logger;
        private readonly Configuration configuration;

        public BlobStorageService(ILogger<BlobStorageService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = Configuration.Load(configuration);
        }

        public async Task<bool> RecoverAsync(BlobDeletedData blobData, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(blobData);

            if (!UrlUtility.IsContainerNamesMatch(blobData.url, configuration.Containers))
            {
                return false;
            }

            var blobClient = new BlobClient(new Uri(blobData.url), new ManagedIdentityCredential());

            try
            {
                //var blobMetadata = await blobClient.GetPropertiesAsync(null, cancellationToken);

                //if (blobMetadata.Value.ContainsMetadata(configuration.MetadataToRecover))
                //{
                var undeleteResponse = await blobClient.UndeleteAsync();
                return !undeleteResponse.IsError;
                //}
                //return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error undeleting blob {blobData.url}");
                return false;
            }
        }
    }

}
