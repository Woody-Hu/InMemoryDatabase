using System;
using System.Collections.Generic;
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
            dataTable.RegisteredType<TestClass>(t => Task.FromResult(t.Id));
            var a = new TestClass() { Id = "1", Name = "aaa", Value = 999 };
            a.SubClasses.Add(new TestSubClass() { Name = "a", Value = 1 });
            a.SubValues.Add(5);
            a.SubValues.Add(7);
            var a2 = await dataTable.InsertEntity(a);

            var x = new TestClass() { Id = "2", Name = "bbb", Value = 20 };
            x.SubClasses.Add(new TestSubClass() { Name = "b", Value = 2 });
            x.SubValues.Add(10);
            x.SubValues.Add(13);
            var x2 = await dataTable.InsertEntity(x);

            var b = await dataTable.GetEntityByIdAsync<TestClass>("1");
            b.Value = 777;

            var b2 = await dataTable.UpdateEntity(b);

            var c = await dataTable.GetEntityByIdAsync<TestClass>("1");

            a2.Value = 666;

            var d = await dataTable.QueryEntities<TestClass>(new QueryItem("Name", QueryOperator.Equal, "aaa"));
            var e = await dataTable.QueryEntities<TestClass>(new QueryItem("Value", QueryOperator.GreaterThan, "30"));
            var f = await dataTable.QueryEntities<TestClass>(new QueryItem("SubClasses", QueryOperator.ArrayContains, "{\"Name\":\"a\", \"Value\": 1}"));
            var g = await dataTable.QueryEntities<TestClass>(new QueryItem("SubClasses", QueryOperator.ArrayContainsPartial, "{\"Name\":\"b\"}"));
            var h = await dataTable.QueryEntities<TestClass>(new QueryItem("SubValues", QueryOperator.ArrayContains, 5.ToString()));
            var i = await dataTable.QueryEntities<TestClass>(new QueryItem("SubValues", QueryOperator.ArrayContainsPartial, 13.ToString()));
            var j = await dataTable.QueryEntities<TestClass>(new QueryItem("Value", QueryOperator.GreaterThanOrEqual, 20.ToString()));
            var k = await dataTable.QueryEntities<TestClass>(new QueryItem("Value", QueryOperator.LessThanOrEqual, 777.ToString()));
            var l = await dataTable.QueryEntities<TestClass>(new QueryItem("Name", QueryOperator.NotEqual, "abc"));
            var m = await dataTable.QueryEntities<TestClass>(new QueryItem("Name", QueryOperator.UpperEqual, "aaa"));
            var n = await dataTable.QueryEntities<TestClass>(new QueryItem("Value", QueryOperator.In, "[20, 35, 42]"));
            Console.WriteLine("Hello World!");
        }

        public class TestClass
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public int Value { get; set; }

            public IList<TestSubClass> SubClasses { get; set; } = new List<TestSubClass>();

            public IList<int> SubValues { get; set; } = new List<int>();

            [JsonProperty("_etag")]
            public string Etag { get; set; }
        }

        public class TestSubClass
        {
            public string Name { get; set; }

            public int Value { get; set; }
        }
    }
}
