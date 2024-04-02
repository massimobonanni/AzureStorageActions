﻿using AzureStorageActions.Console.Commands.GenerateBlobs.Image;
using AzureStorageActions.Console.Commands.GenerateBlobs.Json;
using AzureStorageActions.Console.Commands.GenerateBlobs.Text;
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
        public static BlobData GenerateContent(string blobPrefix, BlobContentType type)
        {
            var data = new BlobData();
            data.Name = GenerateBlobName(blobPrefix, type);
            data.Data = new BinaryData(dataGeneratorsMap[type].Invoke());
            return data;
        }

        private static BinaryData GenerateJsonContent()
        {
            throw new NotImplementedException();
        }

        private static BinaryData GenerateImageContent()
        {
            return new BinaryData(JsonGenerator.Generate());
        }

        private static BinaryData GenerateTextContent()
        {
          
            return new BinaryData(TextGenerator.Generate());
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
