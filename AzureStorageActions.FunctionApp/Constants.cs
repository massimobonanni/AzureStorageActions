using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageActions.FunctionApp
{
    internal static class StorageAccountEvents
    {
        public static string BlobInventoryPolicyCompleted = "Microsoft.Storage.BlobInventoryPolicyCompleted";
        public static string BlobCreated = "Microsoft.Storage.BlobCreated";
    }
}
