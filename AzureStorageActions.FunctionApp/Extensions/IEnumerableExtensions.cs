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
            if (string.IsNullOrEmpty(tagField))
                return false;

            try
            {
                var tagValueColumn = JsonSerializer.Deserialize<IEnumerable<IDictionary<string, string>>>(tagField);

                foreach (var item in tagValueColumn)
                {
                    foreach (var itemTag in item)
                    {
                        if (string.Equals(itemTag.Key, tag.Key, StringComparison.OrdinalIgnoreCase) &&
                                 string.Equals(itemTag.Value, tag.Value, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }
            }
            catch
            {
            }
            
            return false;
        }
    }
}