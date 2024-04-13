using AzureStorageActions.Console.Commands.GenerateBlobs;
using AzureStorageActions.Console.Commands.GenerateFiles;
using System.CommandLine;

var rootCommand = new RootCommand("Azure Storage Actions demo console");
rootCommand.AddCommand(new GenerateBlobsCommand());
rootCommand.AddCommand(new GenerateFilesCommand());

return rootCommand.Invoke(args);
