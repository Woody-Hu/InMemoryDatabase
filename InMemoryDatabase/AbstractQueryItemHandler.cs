using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace InMemoryDatabase
{
    internal abstract class AbstractQueryItemHandler
    {
        internal abstract Func<JToken, bool> TransformQueryExpression(QueryItem queryItem);

        protected JToken GetJTokenField(JToken source, string field)
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
