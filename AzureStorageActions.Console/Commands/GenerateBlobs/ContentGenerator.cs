using AzureStorageActions.Console.Urilities.Image;
using AzureStorageActions.Console.Urilities.Json;
using AzureStorageActions.Console.Urilities.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AzureStorageActions.Console.Commands.GenerateBlobs
{
    internal static class ContentGenerator
    {
        private static Random random = new Random(DateTime.Now.Millisecond);
        public static BlobData GenerateContent(IEnumerable<string> blobPrefixes, IEnumerable<BlobContentType> types)
        {
            var blobPrefix = blobPrefixes.ElementAt(random.Next(0, blobPrefixes.Count()));
            var type = types.ElementAt(random.Next(0, types.Count()));

            var data = new BlobData();
            data.Name = GenerateBlobName(blobPrefix, type);
            data.Data = new BinaryData(dataGeneratorsMap[type].Invoke());
            return data;
        }

        private static string GenerateBlobName(string blobPrefix, BlobContentType type)
        {
            string extension = extensionsMap[type];
            return $"{blobPrefix}{Guid.NewGuid()}{extension}";
        }

        private static IDictionary<BlobContentType, string> extensionsMap = new Dictionary<BlobContentType, string>()
        {
            {BlobContentType.Text,".txt" },
            {BlobContentType.Image,".jpg" },
            { BlobContentType.Json,".json"}
        };

        private static IDictionary<BlobContentType, Func<byte[]>> dataGeneratorsMap= new Dictionary<BlobContentType, Func<byte[]>>()
        {
            {BlobContentType.Text, TextGenerator.Generate },
            {BlobContentType.Image, ImageGenerator.Generate },
            {BlobContentType.Json, JsonGenerator.Generate }
        };

        


    }
}
