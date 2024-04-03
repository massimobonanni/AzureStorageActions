using AzureStorageActions.FunctionApp.Interfaces;
using AzureStorageActions.FunctionApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddScoped<IBlobRecoverer, BlobStorageService>();
        services.AddScoped<IBlobTagger, BlobStorageTaggerService>();
        services.AddScoped<IBlobInventoryAnalyzer, BlobStorageInventoryService>();
    })
    .Build();

host.Run();
