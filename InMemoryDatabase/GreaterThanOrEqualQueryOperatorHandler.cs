using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace InMemoryDatabase
{
    [QueryItemHandler(QueryOperator.GreaterThanOrEqual)]
    internal class GreaterThanOrEqualQueryOperatorHandler:AbstractQueryItemHandler
    {
        internal override Func<JToken, bool> TransformQueryExpression(QueryItem queryItem)
        {
            return token =>
            {
                var currentObj = GetJTokenField(token, queryItem.Field);
                return currentObj != null && (currentObj.Type == JTokenType.Integer || currentObj.Type == JTokenType.Float) &&
                       double.TryParse(queryItem.Value, out var value) && currentObj.Value<double>() >= value;
            };
        }
    }
}
