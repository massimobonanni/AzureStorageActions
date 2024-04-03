using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageActions.FunctionApp.Models
{
    public class BlobInventoryPolicyCompletedData
    {
        public DateTime scheduleDateTime { get; set; }
        public string accountName { get; set; }
        public string ruleName { get; set; }
        public string policyRunStatus { get; set; }
        public string policyRunStatusMessage { get; set; }
        public string policyRunId { get; set; }
        public string manifestBlobUrl { get; set; }
    }
}
