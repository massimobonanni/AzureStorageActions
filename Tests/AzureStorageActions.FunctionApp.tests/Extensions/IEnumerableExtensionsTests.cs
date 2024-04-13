using System;
using System.Collections.Generic;
using Xunit;
using System.Text.Json;

namespace System.Collections.Generic.Tests
{
    public class IEnumerableExtensionsTests
    {
        #region "ContainsTag method"
        [Fact]
        public void ContainsTag_ShouldReturnTrue_WhenTagExists()
        {
            // Arrange
            var source = new List<string>
            {
                "",
                "string",
                "[{\"todelete\":\"true\"}]"
            };
            var tag = new KeyValuePair<string, string>("toDelete", "true");

            // Act
            var result = source.ContainsTag(2, tag);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ContainsTag_ShouldReturnFalse_WhenTagDoesNotExist()
        {
            // Arrange
            var source = new List<string>
            {
                JsonSerializer.Serialize(new Dictionary<string, string> { { "key1", "value1" } }),
                JsonSerializer.Serialize(new Dictionary<string, string> { { "key2", "value2" } }),
                JsonSerializer.Serialize(new Dictionary<string, string> { { "key3", "value3" } })
            };
            var tag = new KeyValuePair<string, string>("key4", "value4");

            // Act
            var result = source.ContainsTag(1, tag);

            // Assert
            Assert.False(result);
        }
        #endregion "ContainsTag method"
    }
}
