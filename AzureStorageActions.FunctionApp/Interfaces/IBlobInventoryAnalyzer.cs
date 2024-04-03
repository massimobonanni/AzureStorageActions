using AzureStorageActions.FunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageActions.FunctionApp.Interfaces
{
    public interface IBlobInventoryAnalyzer
    {
        Task<bool> AnalyzeAsync(BlobInventoryPolicyCompletedData inventoryData, CancellationToken cancellationToken = default);
    }
    
}
