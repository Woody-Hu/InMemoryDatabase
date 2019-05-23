using System;
using System.Threading.Tasks;
using InMemoryDatabase;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var dataTable = new InMemoryDataTable();
            dataTable.RegisteredType<TestClass>(t =>Task.FromResult(t.Id));
            var a = new TestClass() {Id = "1", Name = "aaa", Value = "999"};
            var a2 = await dataTable.InsertEntity(a);

            var b = await dataTable.GetEntityByIdAsync<TestClass>("1");
            b.Value = "777";

            var b2 = await dataTable.UpdateEntity(b);

            var c = await dataTable.GetEntityByIdAsync<TestClass>("1");

            a2.Value = "666";


            Console.WriteLine("Hello World!");
        }

        public class TestClass
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string Value { get; set; }

            [JsonProperty("_etag")]
            public string Etag { get; set; }
        }
    }
}
