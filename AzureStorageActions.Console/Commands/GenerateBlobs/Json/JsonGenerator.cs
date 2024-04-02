using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureStorageActions.Console.Commands.GenerateBlobs.Json
{
    internal static class JsonGenerator
    {
        private static string[] products = new[] { "Apple", "Banana", "Orange", "Strawberry", "Grape" };

        private static Faker<Order> orderPrototipe = new Faker<Order>()
                .StrictMode(true)
                .RuleFor(o => o.OrderId, f => new Guid())
                .RuleFor(o => o.Item, f => f.PickRandom(products))
                .RuleFor(o => o.Quantity, f => f.Random.Number(1, 10))
                .RuleFor(o => o.LotNumber, f => f.Random.Int(0, 100).OrNull(f, .8f));

        private static Faker<User> userPrototipe = new Faker<User>()
            .CustomInstantiator(f => new User(f.Random.Int(), f.Random.Replace("###-##-####")))
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Avatar, f => f.Internet.Avatar())
            .RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
            .RuleFor(u => u.SomethingUnique, f => $"Value {f.UniqueIndex}")
            .RuleFor(u => u.SomeGuid, Guid.NewGuid)
            .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
            .RuleFor(u => u.CartId, f => Guid.NewGuid())
            .RuleFor(u => u.FullName, (f, u) => u.FirstName + " " + u.LastName)
            .RuleFor(u => u.Orders, f => orderPrototipe.Generate(f.Random.Int(1, 10)));

        public static byte[] Generate()
        {
            var user = userPrototipe.Generate();

            var jsonUser = JsonSerializer.Serialize<User>(user,
                new JsonSerializerOptions()
                {
                    WriteIndented = true,
                });

            return Encoding.UTF8.GetBytes(jsonUser);
        }
    }
}
