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

namespace AzureStorageActions.Console.Commands.GenerateFiles
{
    internal static class ContentGenerator
    {
        private static Random random = new Random(DateTime.Now.Millisecond);
        public static FileData GenerateContent(IEnumerable<string> filePrefixes, IEnumerable<FileContentType> types)
        {
            string filePrefix = "";
            if (filePrefixes!=null && filePrefixes.Any())
                filePrefix = filePrefixes.ElementAt(random.Next(0, filePrefixes.Count()));
            var type = types.ElementAt(random.Next(0, types.Count()));

            var data = new FileData();
            data.Name = GenerateBlobName(filePrefix, type);
            data.Data = dataGeneratorsMap[type].Invoke();
            return data;
        }

        private static string GenerateBlobName(string blobPrefix, FileContentType type)
        {
            string extension = extensionsMap[type];
            return $"{blobPrefix}{Guid.NewGuid()}{extension}";
        }

        private static IDictionary<FileContentType, string> extensionsMap = new Dictionary<FileContentType, string>()
        {
            {FileContentType.Text,".txt" },
            {FileContentType.Image,".jpg" },
            {FileContentType.Json,".json"}
        };

        private static IDictionary<FileContentType, Func<byte[]>> dataGeneratorsMap= new Dictionary<FileContentType, Func<byte[]>>()
        {
            {FileContentType.Text, TextGenerator.Generate },
            {FileContentType.Image, ImageGenerator.Generate },
            {FileContentType.Json, JsonGenerator.Generate }
        };

        


    }
}
