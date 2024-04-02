namespace AzureStorageActions.FunctionApp.Models
{
    public class BlobDeletedData
    {
        public class StorageDiagnostics
        {
            public string batchId { get; set; }
        }

        public string api { get; set; }
        public string requestId { get; set; }
        public string eTag { get; set; }
        public string contentType { get; set; }
        public int contentLength { get; set; }
        public string blobType { get; set; }
        public string url { get; set; }
        public string sequencer { get; set; }
        public StorageDiagnostics storageDiagnostics { get; set; }
    }


}
