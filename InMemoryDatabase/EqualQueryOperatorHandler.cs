using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace InMemoryDatabase
{
    [QueryItemHandler(QueryOperator.Equal)]
    internal class EqualQueryOperatorHandler : AbstractQueryItemHandler
    {
        internal override Func<JToken, bool> TransformQueryExpression(QueryItem queryItem)
        {
            return token =>
            {
                var currentObj = GetJTokenField(token, queryItem.Field);
                return currentObj != null && currentObj.ToString() == queryItem.Value;
            };
        }
    }
}
