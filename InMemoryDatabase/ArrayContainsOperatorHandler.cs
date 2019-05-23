using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace InMemoryDatabase
{
    [QueryItemHandler(QueryOperator.ArrayContains)]
    internal class ArrayContainsOperatorHandler : AbstractQueryItemHandler
    {
        internal override Func<JToken, bool> TransformQueryExpression(QueryItem queryItem)
        {
            return token =>
            {
                var currentObj = GetJTokenField(token, queryItem.Field);
                var value = JToken.Parse(queryItem.Value);
                return currentObj != null && currentObj.Type == JTokenType.Array && currentObj is JArray array && array.Any(t => JToken.DeepEquals(t, value));
            };
        }
    }
}
