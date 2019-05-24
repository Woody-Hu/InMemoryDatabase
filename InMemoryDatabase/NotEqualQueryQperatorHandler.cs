using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace InMemoryDatabase
{
    [QueryItemHandler(QueryOperator.NotEqual)]
    internal class NotEqualQueryOperatorHandler : AbstractQueryItemHandler
    {
        internal override Func<JToken, bool> TransformQueryExpression(QueryItem queryItem)
        {
            return token =>
            {
                var currentObj = GetJTokenField(token, queryItem.Field);
                return currentObj != null && currentObj.ToString() != queryItem.Value;
            };
        }
    }
}
