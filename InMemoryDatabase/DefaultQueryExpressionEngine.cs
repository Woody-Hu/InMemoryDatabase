using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;

namespace InMemoryDatabase
{
    public class DefaultQueryExpressionEngine : IQueryExpressionEngine
    {
        private readonly ConcurrentDictionary<QueryOperator, Func<QueryItem, Func<JToken, bool>>> _handlerDictionary = new ConcurrentDictionary<QueryOperator, Func<QueryItem, Func<JToken, bool>>>();

        public DefaultQueryExpressionEngine()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var types = currentAssembly.GetTypes();
            var baseType = typeof(AbstractQueryItemHandler);
            var attributeType = typeof(QueryItemHandlerAttribute);
            var handlerTypes = types.Where(k => baseType.IsAssignableFrom(k) && !k.IsAbstract && k.GetConstructors().Any(j => j.GetParameters().Length == 0));
            foreach (var oneHandlerType in handlerTypes)
            {
                var attribute = oneHandlerType.GetCustomAttribute(attributeType) as QueryItemHandlerAttribute;
                if (attribute == null)
                {
                    continue;
                }

                var handler = Activator.CreateInstance(oneHandlerType) as AbstractQueryItemHandler;
                if (handler == null)
                {
                    continue;
                }

                _handlerDictionary.TryAdd(attribute.QueryOperator, handler.TransformQueryExpression);
            }
        }

        public Func<JToken, bool> TransformQueryExpression(QueryItem queryItem)
        {
            if (_handlerDictionary.ContainsKey(queryItem.QueryOperator))
            {
                return _handlerDictionary[queryItem.QueryOperator](queryItem);
            }

            return null;
        }
    }
}
