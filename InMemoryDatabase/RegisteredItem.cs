using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace InMemoryDatabase
{
    internal sealed class RegisteredItem<T> : IRegisteredItem
        where T : class
    {
        internal Func<T, Task<JToken>> ForwardFunc { get; }

        internal Func<JToken, Task<T>> BackwardFunc { get; }

        internal Func<T, Task<string>> GetIdFunc { get; }

        public Type Type { get; }

        internal RegisteredItem(Func<T, Task<JToken>> forwardFunc, Func<JToken, Task<T>> backwardFunc,
            Func<T, Task<string>> getIdFunc)
        {
            Type = typeof(T);
            ForwardFunc = forwardFunc;
            BackwardFunc = backwardFunc;
            GetIdFunc = getIdFunc;
            if (ForwardFunc == null || BackwardFunc == null || GetIdFunc == null)
            {
                throw new ArgumentException();
            }
        }
    }
}
