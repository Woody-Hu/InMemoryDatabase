using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace InMemoryDatabase
{
    [QueryItemHandler(QueryOperator.In)]
    internal class InOperatorHandler:AbstractQueryItemHandler
    {
        internal override Func<JToken, bool> TransformQueryExpression(QueryItem queryItem)
        {
            return token =>
            {
                var currentObj = GetJTokenField(token, queryItem.Field);
                var value = JToken.Parse(queryItem.Value);
                return currentObj != null && value.Type == JTokenType.Array && value.Contains(currentObj, new DefaultDeepEqualityComparer());
            };
        }

        private class DefaultDeepEqualityComparer : IEqualityComparer<JToken>
        {
            public bool Equals(JToken x, JToken y)
            {
                return JToken.DeepEquals(x, y);
            }

            public int GetHashCode(JToken obj)
            {
                return obj.ToString().GetHashCode();
            }
        }
    }
}
