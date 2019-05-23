using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InMemoryDatabase
{
    public class InMemoryDataTable
    {
        private readonly ConcurrentDictionary<string, JToken> _concurrentDictionary = new ConcurrentDictionary<string, JToken>();

        private readonly ConcurrentDictionary<Type, IRegisteredItem> _concurrentRegisteredItemDictionary = new ConcurrentDictionary<Type, IRegisteredItem>();

        private readonly IQueryExpressionEngine _queryExpressionEngine = new DefaultQueryExpressionEngine();

        public void RegisteredType<T>(Func<T, Task<string>> getIdFunc)
            where T : class
        {
            var type = typeof(T);
            var item = new RegisteredItem<T>(t => Task.FromResult(JToken.Parse(JsonConvert.SerializeObject(t))), j => Task.FromResult(JsonConvert.DeserializeObject<T>(j.ToString())), getIdFunc);
            _concurrentRegisteredItemDictionary.GetOrAdd(type, item);
        }

        public async Task<T> GetEntityByIdAsync<T>(string id)
            where T : class
        {
            var type = typeof(T);
            var item = _concurrentRegisteredItemDictionary[type] as RegisteredItem<T>;
            var jToken = _concurrentDictionary[id];
            if (jToken == null)
            {
                return null;
            }

            var res = await item.BackwardFunc(jToken);
            return res;
        }

        public async Task<T> InsertEntity<T>(T entity)
            where T : class
        {
            var type = typeof(T);
            var item = _concurrentRegisteredItemDictionary[type] as RegisteredItem<T>;
            var id = await item.GetIdFunc(entity);
            if (_concurrentDictionary.ContainsKey(id))
            {
                throw new ArgumentException();
            }

            var jToken = await item.ForwardFunc(entity);
            jToken["_etag"] = Guid.NewGuid().ToString();

            _concurrentDictionary.AddOrUpdate(id, jToken, (s, token) => throw new ArgumentException());
            jToken = _concurrentDictionary[id];

            return await item.BackwardFunc(jToken);
        }

        public async Task<T> UpdateEntity<T>(T entity)
            where T : class
        {
            var type = typeof(T);
            var item = _concurrentRegisteredItemDictionary[type] as RegisteredItem<T>;
            var id = await item.GetIdFunc(entity);
            if (!_concurrentDictionary.ContainsKey(id))
            {
                throw new ArgumentException();
            }

            var existed = _concurrentDictionary[id];
            var jToken = await item.ForwardFunc(entity);
            var needCheckEtag = jToken["_etag"] != null;
            var currentEtag = jToken["_etag"]?.ToString();
            var localToken = jToken;
            jToken = _concurrentDictionary.AddOrUpdate(id, jToken, (s, token) =>
            {
                if (needCheckEtag && existed["_etag"].ToString() != currentEtag)
                {
                    throw new ArgumentException();
                }

                localToken["_etag"] = Guid.NewGuid().ToString();
                return localToken;
            });

            return await item.BackwardFunc(jToken);
        }

        public async Task<IEnumerable<T>> QueryEntities<T>(QueryItem queryItem)
            where T : class
        {
            var type = typeof(T);
            var item = _concurrentRegisteredItemDictionary[type] as RegisteredItem<T>;
            var predict = _queryExpressionEngine.TransformQueryExpression(queryItem);
            var jTokens = _concurrentDictionary.Where(k => predict(k.Value));
            var list = new List<T>();

            foreach (var jToken in jTokens)
            {
                var obj = await item.BackwardFunc(jToken.Value);
                list.Add(obj);
            }

            return list;
        }
    }
}
