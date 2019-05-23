using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace InMemoryDatabase
{
    public class DefaultQueryExpressionEngine : IQueryExpressionEngine
    {
        public Func<JToken, bool> TransformQueryExpression(QueryItem queryItem)
        {
            if (queryItem.QueryOperator == QueryOperator.Equal)
            {
                return token =>
                {
                    var currentObj = GetJTokenField(token, queryItem.Field);
                    return currentObj != null && currentObj.ToString() == queryItem.Value;
                };
            }
            else if (queryItem.QueryOperator == QueryOperator.LessThan)
            {
                return token =>
                {
                    var currentObj = GetJTokenField(token, queryItem.Field);
                    return currentObj != null && (currentObj.Type == JTokenType.Integer || currentObj.Type == JTokenType.Float) &&
                           double.TryParse(queryItem.Value, out var value) && currentObj.Value<double>() < value;
                };
            }
            else if (queryItem.QueryOperator == QueryOperator.GreaterThan)
            {
                return token =>
                {
                    var currentObj = GetJTokenField(token, queryItem.Field);
                    return currentObj != null && (currentObj.Type == JTokenType.Integer || currentObj.Type == JTokenType.Float) &&
                           double.TryParse(queryItem.Value, out var value) && currentObj.Value<double>() > value;
                };
            }
            else if (queryItem.QueryOperator == QueryOperator.ArrayContains)
            {
                return token =>
                {
                    var currentObj = GetJTokenField(token, queryItem.Field);
                    var value = JToken.Parse(queryItem.Value);
                    return currentObj != null && currentObj.Type == JTokenType.Array && currentObj is JArray array && array.Any(t => JToken.DeepEquals(t, value));
                };
            }
            else if (queryItem.QueryOperator == QueryOperator.ArrayContainsPartial)
            {
                return token =>
                {
                    var currentObj = GetJTokenField(token, queryItem.Field);
                    var valueObject = JToken.Parse(queryItem.Value);
                    return currentObj != null && currentObj.Type == JTokenType.Array && currentObj is JArray array
                           && array.Any(t => JTokenPartialEqual(t, valueObject));
                };
            }


            return null;
        }

        private bool JTokenPartialEqual(JToken fullJToken, JToken partialJToken)
        {
            if (fullJToken is JObject fullJObject && partialJToken is JObject partialJObject )
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

        private JToken GetJTokenField(JToken source, string field)
        {
            var fields = field.Split('.');
            var currentObj = source;
            foreach (var oneField in fields)
            {
                currentObj = currentObj[oneField];
                if (currentObj == null)
                {
                    return null;
                }
            }

            return currentObj;
        }
    }
}
