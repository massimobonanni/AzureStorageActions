using Azure.Storage.Blobs.Models;
using AzureStorageActions.FunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Storage.Blobs.Models
{
    internal static class BlobPropertiesExtensions
    {
        public static bool ContainsMetadata(this BlobProperties properties, IEnumerable<BlobMetadata> metadata)
        {
            if (metadata == null)
            {
                return false;
            }

            foreach (var pair in properties.Metadata)
            {
                if (metadata.Any(m => string.Compare(m.Key, pair.Key,true)==0 && string.Compare(m.Value, pair.Value,true)==0))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
