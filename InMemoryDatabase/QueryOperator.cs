using System;
using System.Collections.Generic;
using System.Text;

namespace InMemoryDatabase
{
    public enum QueryOperator
    {
        Equal,
        NotEqual,
        UpperEqual,
        LessThan,
        GreaterThan,
        ArrayContains,
        ArrayContainsPartial,
        LessThanOrEqual,
        GreaterThanOrEqual
    }
}
