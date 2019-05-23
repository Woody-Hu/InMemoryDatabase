using System;
using System.Collections.Generic;
using System.Text;

namespace InMemoryDatabase
{
    public class QueryItem
    {
        public string Field { get; }

        public QueryOperator QueryOperator { get; }


        public string Value { get; }

        public QueryItem(string field, QueryOperator queryOperator, string value)
        {
            Field = field;
            QueryOperator = queryOperator;
            Value = value;
        }
    }
}
