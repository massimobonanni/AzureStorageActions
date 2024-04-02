using AzureStorageActions.Console.Commands.GenerateBlobs;
using System.CommandLine;

var rootCommand = new RootCommand("Azure Storage Actions demo console");
rootCommand.AddCommand(new GenerateBlobsCommand());

return rootCommand.Invoke(args);
