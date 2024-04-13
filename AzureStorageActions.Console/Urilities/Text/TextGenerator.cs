using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageActions.Console.Urilities.Text
{
    internal static class TextGenerator
    {
        public static byte[] Generate()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            int size = random.Next(100, 1000);
            return Encoding.UTF8.GetBytes(Faker.Lorem.Sentence(size));
        }
    }
}
