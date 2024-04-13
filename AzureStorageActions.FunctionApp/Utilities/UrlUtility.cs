using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageActions.FunctionApp.Utilities
{
    internal static class UrlUtility
    {
        public static bool IsContainerNameMatch(string url, string containerName)
        {
            Uri uri = new Uri(url);
            string[] segments = uri.AbsolutePath.Split('/');

            // The first segment will be empty because the path starts with a slash
            // The second segment is the container name
            if (segments.Length > 2)
            {
                return string.Compare(segments[1], containerName, true) == 0;
            }

            return false;
        }

        public static bool IsContainerNamesMatch(string url, IEnumerable<string> containerNames)
        {
            Uri uri = new Uri(url);
            string[] segments = uri.AbsolutePath.Split('/');

            // The first segment will be empty because the path starts with a slash
            // The second segment is the container name
            if (segments.Length > 2)
            {
                return containerNames.Any(containerNames => string.Compare(segments[1], containerNames, true) == 0);
            }

            return false;
        }

        public static string ExtractStorageName(string url)
        {
            Uri uri = new Uri(url);
            return uri.Host.Split('.').FirstOrDefault();
        }

        public static bool IsBlobNameStartsWith(string url, string prefix)
        {
            Uri uri = new Uri(url);
            string[] segments = uri.AbsolutePath.Split('/');

            // The first segment will be empty because the path starts with a slash
            // The third segment is the blob name
            if (segments.Length >= 3)
            {
                return segments.Last().StartsWith(prefix);
            }

            return false;
        }
    }
}
