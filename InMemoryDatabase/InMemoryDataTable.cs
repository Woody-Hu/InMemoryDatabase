using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace InMemoryDatabase
{
    public class InMemoryDataTable
    {
        private ConcurrentDictionary<string, JToken> _concurrentDictionary = new ConcurrentDictionary<string, JToken>();

        private ConcurrentDictionary<Type, IRegisteredItem> _concurrentRegisteredItemDictionary = new ConcurrentDictionary<Type, IRegisteredItem>();

        public void RegisteredType<T>(Func<T, Task<JToken>> forwardFunc, Func<JToken, Task<T>> backwardFunc, Func<T, Task<string>> getIdFunc)
            where T : class
        {
            var type = typeof(T);
            var item = new RegisteredItem<T>(forwardFunc, backwardFunc, getIdFunc);
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
    }
}
