using Xunit;
using AzureStorageActions.FunctionApp.Utilities;
using System.Collections.Generic;

namespace AzureStorageActions.FunctionApp.Tests
{
    public class UrlUtilityTests
    {
        #region IsContainerNameMatch method
        [Fact]
        public void IsContainerNameMatch_ShouldReturnTrue_WhenContainerNameMatches()
        {
            string url = "https://storageaccount.blob.core.windows.net/containername/blobname";
            string containerName = "containername";

            bool result = UrlUtility.IsContainerNameMatch(url, containerName);

            Assert.True(result);
        }
        #endregion IsContainerNameMatch method

        #region IsContainerNamesMatch method
        [Fact]
        public void IsContainerNamesMatch_ShouldReturnTrue_WhenContainerNameMatches()
        {
            string url = "https://storageaccount.blob.core.windows.net/containername/blobname";
            IEnumerable<string> containerNames = new List<string> { "containername" };

            bool result = UrlUtility.IsContainerNamesMatch(url, containerNames);

            Assert.True(result);
        }
        #endregion IsContainerNamesMatch method

        #region ExtractStorageName method
        [Fact]
        public void ExtractStorageName_ShouldReturnContainerName_WhenUrlIsValid()
        {
            string url = "https://storageaccount.blob.core.windows.net/containername/blobname";

            string result = UrlUtility.ExtractStorageName(url);

            Assert.Equal("storageaccount", result);
        }
        #endregion ExtractStorageName method

        #region IsBlobNameStartsWith method
        [Theory]
        [MemberData(nameof(UrlUtilityTestData.BlobNameStartsWithData), 
            MemberType = typeof(UrlUtilityTestData))]
        public void IsBlobNameStartsWith_ShouldReturnTrue_WhenBlobNameStartsWithPrefix(string url, string prefix)
        {
            bool result = UrlUtility.IsBlobNameStartsWith(url, prefix);

            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(UrlUtilityTestData.BlobNameDoesNotStartsWithData),
            MemberType = typeof(UrlUtilityTestData))]
        public void IsBlobNameStartsWith_ShouldReturnFalse_WhenBlobNameDoesNotStartsWithPrefix(string url, string prefix)
        {
            bool result = UrlUtility.IsBlobNameStartsWith(url, prefix);

            Assert.False(result);
        }
        #endregion IsBlobNameStartsWith method
    }

    public class UrlUtilityTestData
    {
        public static IEnumerable<object[]> BlobNameStartsWithData()
        {
            yield return new object[] { "https://storageaccount.blob.core.windows.net/containername/blobname", "blob" };
            yield return new object[] { "https://storageactionsdata.blob.core.windows.net/blobtagging-function/invoicee09ae2e1-6027-4481-8059-bf409c857d4a.json", "invoice" };
            yield return new object[] { "https://storageactionsdata.blob.core.windows.net/blobtagging-function/reportd7ac2c62-dcb2-41db-917b-f9d0051cf041.txt", "report" };
            yield return new object[] { "https://storageactionsdata.blob.core.windows.net/blobtagging-function/folder/reportd7ac2c62-dcb2-41db-917b-f9d0051cf041.txt", "report" };
            yield return new object[] { "https://storageactionsdata.blob.core.windows.net/blobtagging-function/folder/subfolder/reportd7ac2c62-dcb2-41db-917b-f9d0051cf041.txt", "report" };
        }

        public static IEnumerable<object[]> BlobNameDoesNotStartsWithData()
        {
            yield return new object[] { "https://storageaccount.blob.core.windows.net/containername/blobname", "name" };
            yield return new object[] { "https://storageactionsdata.blob.core.windows.net/blobtagging-function/invoicee09ae2e1-6027-4481-8059-bf409c857d4a.json", "report" };
            yield return new object[] { "https://storageactionsdata.blob.core.windows.net/blobtagging-function/reportd7ac2c62-dcb2-41db-917b-f9d0051cf041.txt", "invoice" };
            yield return new object[] { "https://storageactionsdata.blob.core.windows.net/blobtagging-function/folder/reportd7ac2c62-dcb2-41db-917b-f9d0051cf041.txt", "receipt" };
            yield return new object[] { "https://storageactionsdata.blob.core.windows.net/blobtagging-function/folder/subfolder/reportd7ac2c62-dcb2-41db-917b-f9d0051cf041.txt", "invoice" };
        }
    }
}
