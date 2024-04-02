using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageActions.Console.Commands.GenerateBlobs
{
    internal class BlobData
    {
        public string Name { get; set; }

        public BinaryData Data { get; set; }
    }
}
