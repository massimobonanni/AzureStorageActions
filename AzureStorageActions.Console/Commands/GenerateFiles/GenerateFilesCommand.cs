using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageActions.Console.Commands.GenerateFiles
{

    /// <summary>
    /// Represents a command that generates a set of random blobs in a storage account.
    /// </summary>
    /// <example>
    ///     asa.exe genfiles --connection-string "DefaultEndpointsProtocol=https;AccountName=youraccount;AccountKey=yourkey" --number-of-blobs 10 --container-name "yourcontainer" --blob-content-type "text" --blob-prefix "prod"
    /// </example>
    internal class GenerateFilesCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateFilesCommand"/> class.
        /// </summary>
        public GenerateFilesCommand() :
            base("genfiles", "Generates a set of random files in a folder")
        {
            var pathOption = new Option<string>(
                new string[] { "--path", "-p" },
                "The folder path")
            {
                IsRequired = true
            };
            this.AddOption(pathOption);

            var numberOfFilesOption = new Option<int>(
                new string[] { "--number-of-files", "-n" },
                "The number of files to generate")
            {
                IsRequired = true
            };
            this.AddOption(numberOfFilesOption);

            var fileContentTypeOptions = new Option<IEnumerable<FileContentType>>(
                new string[] { "--file-content-type", "-ct" },
                () => new FileContentType[] { FileContentType.Text },
                "The content type to use for the files. Valid values: Text, Json, Jpeg")
            {
                IsRequired = false
            };
            this.AddOption(fileContentTypeOptions);

            var filePrefixOption = new Option<IEnumerable<string>>(
                new string[] { "--file-prefix", "-fp" },
                "The prefix to use for the file names")
            {
                IsRequired = false
            };
            this.AddOption(filePrefixOption);

            this.SetHandler(CommandHandler, pathOption, numberOfFilesOption, fileContentTypeOptions, filePrefixOption);
        }

        /// <summary>
        /// Handles the execution of the generate files command.
        /// </summary>
        /// <param name="filePath">The folder path where the files will be generated.</param>
        /// <param name="numberOfFiles">The number of files to generate.</param>
        /// <param name="contentTypes">The content types to use for the files.</param>
        /// <param name="blobPrefixes">The prefixes to use for the file names.</param>
        private async Task CommandHandler(string filePath, int numberOfFiles,
            IEnumerable<FileContentType> contentTypes, IEnumerable<string> blobPrefixes)
        {
            System.Console.WriteLine($"Generating {numberOfFiles} file in {filePath}");

            // Upload numberOfBlobs blobs
            for (int i = 0; i < numberOfFiles; i++)
            {
                var fileData = ContentGenerator.GenerateContent(blobPrefixes, contentTypes);
                var fullFileName=Path.Combine(filePath,fileData.Name);
                System.Console.WriteLine($"Generating file {fileData.Name}");
                await File.WriteAllBytesAsync(fullFileName, fileData.Data);  
                System.Console.WriteLine($"File {fileData.Name} generated");
            }
        }
    }
}
