using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    internal static class IEnumerableExtensions
    {
        public static bool ContainsTag(this IEnumerable<string> source,
            int index, KeyValuePair<string, string> tag)
        {
            var tagField = source.ElementAt(index);
            var tagValueColumn = new Dictionary<string, string>(
                JsonSerializer.Deserialize<Dictionary<string, string>>(tagField),
                StringComparer.OrdinalIgnoreCase);

            return tagValueColumn.ContainsKey(tag.Key) && 
                tagValueColumn[tag.Key].Equals(tag.Value,StringComparison.OrdinalIgnoreCase);
        }
    }
}
