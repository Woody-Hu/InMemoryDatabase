using System;
using System.Collections.Generic;
using System.Text;

namespace InMemoryDatabase
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal class QueryItemHandlerAttribute:Attribute
    {
        internal QueryOperator QueryOperator { get; }

        internal QueryItemHandlerAttribute(QueryOperator queryOperator)
        {
            QueryOperator = queryOperator;
        }
    }
}
