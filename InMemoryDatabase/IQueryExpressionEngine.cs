using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace InMemoryDatabase
{
    internal interface IQueryExpressionEngine
    {
        Func<JToken, bool> TransformQueryExpression(QueryItem queryItem);
    }
}
