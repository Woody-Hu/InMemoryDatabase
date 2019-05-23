using System;
using System.Collections.Generic;
using System.Text;

namespace InMemoryDatabase
{
    internal interface IRegisteredItem
    {
        Type Type { get; }
    }
}
