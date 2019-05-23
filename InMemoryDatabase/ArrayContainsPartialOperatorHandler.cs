using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace InMemoryDatabase
{
    [QueryItemHandler(QueryOperator.ArrayContainsPartial)]
    internal class ArrayContainsPartialOperatorHandler : AbstractQueryItemHandler
    {
        internal override Func<JToken, bool> TransformQueryExpression(QueryItem queryItem)
        {
            return token =>
            {
                var currentObj = GetJTokenField(token, queryItem.Field);
                var valueObject = JToken.Parse(queryItem.Value);
                return currentObj != null && currentObj.Type == JTokenType.Array && currentObj is JArray array
                       && array.Any(t => JTokenPartialEqual(t, valueObject));
            };
        }

        private bool JTokenPartialEqual(JToken fullJToken, JToken partialJToken)
        {
            if (fullJToken is JObject fullJObject && partialJToken is JObject partialJObject)
            {
                foreach (var oneChild in partialJObject.Children<JProperty>())
                {
                    var fullJObjectChildren = fullJObject[oneChild.Name];
                    if (fullJObjectChildren == null || !JToken.DeepEquals(oneChild.Value, fullJObjectChildren))
                    {
                        return false;
                    }
                }

                return true;
            }

            return JToken.DeepEquals(fullJToken, partialJToken);
        }
    }
}
