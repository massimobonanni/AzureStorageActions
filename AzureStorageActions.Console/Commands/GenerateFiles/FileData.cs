using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageActions.Console.Commands.GenerateFiles
{
    internal class FileData
    {
        public string Name { get; set; }

        public byte[] Data { get; set; }
    }
}
