using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageActions.Console.Commands.GenerateBlobs
{

    /// <summary>
    /// Represents a command that generates a set of random blobs in a storage account.
    /// </summary>
    /// <example>
    ///     asa.exe genblobs --connection-string "DefaultEndpointsProtocol=https;AccountName=youraccount;AccountKey=yourkey" --number-of-blobs 10 --container-name "yourcontainer" --blob-content-type "text" --blob-prefix "prod"
    /// </example>
    internal class GenerateBlobsCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateBlobsCommand"/> class.
        /// </summary>
        public GenerateBlobsCommand() :
            base("genblobs", "Generates a set of random blobs in a storage account")
        {
            var storageConnectionStringOption = new Option<string>(
                new string[] { "--connection-string", "-c" },
                "The connection string to the storage account")
            {
                IsRequired = true
            };
            this.AddOption(storageConnectionStringOption);

            var numberOfBlobsOption = new Option<int>(
                new string[] { "--number-of-blobs", "-n" },
                "The number of blobs to generate")
            {
                IsRequired = true
            };
            this.AddOption(numberOfBlobsOption);

            var containerNameOption = new Option<string>(
                new string[] { "--container-name", "-cn" },
                "The name of the container to generate the blobs in")
            {
                IsRequired = true
            };
            this.AddOption(containerNameOption);

            var blobContentTypeOptions = new Option<BlobContentType>(
                new string[] { "--blob-content-type", "-ct" },
                () => BlobContentType.Text,
                "The content type to use for the blobs")
            {
                IsRequired = false
            };
            this.AddOption(blobContentTypeOptions);

            var blobPrefixOption = new Option<string>(
                new string[] { "--blob-prefix", "-bp" },
                "The prefix to use for the blob names")
            {
                IsRequired = false
            };
            this.AddOption(blobPrefixOption);

            this.SetHandler(async (connectionString, numberOfBlobs, containerName, contentType, blobPrefix) =>
            {
                await CommandHandler(connectionString, numberOfBlobs, containerName, contentType, blobPrefix);
            }, storageConnectionStringOption, numberOfBlobsOption, containerNameOption, blobContentTypeOptions, blobPrefixOption);
        }

        /// <summary>
        /// Handles the command execution.
        /// </summary>
        /// <param name="connectionString">The connection string to the storage account.</param>
        /// <param name="numberOfBlobs">The number of blobs to generate.</param>
        /// <param name="containerName">The name of the container to generate the blobs in.</param>
        /// <param name="contentType">The content type to use for the blobs.</param>
        /// <param name="blobPrefix">The prefix to use for the blob names.</param>
        private async Task CommandHandler(string connectionString, int numberOfBlobs, string containerName, BlobContentType contentType, string blobPrefix)
        {
            System.Console.WriteLine($"Generating {numberOfBlobs} blobs of type {contentType} in container {containerName} with prefix {blobPrefix}");

            // Create a ContainerService client to access the container in the Azure Storage account
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Upload numberOfBlobs blobs
            for (int i = 0; i < numberOfBlobs; i++)
            {
                var blobData= ContentGenerator.GenerateContent(blobPrefix,contentType);

                BlobClient blobClient = containerClient.GetBlobClient(blobData.Name);
                await blobClient.UploadAsync(blobData.Data);
            }

        }
    }
}
