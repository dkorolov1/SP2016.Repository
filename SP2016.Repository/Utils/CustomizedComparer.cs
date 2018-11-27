using System;
using System.Collections.Generic;

namespace SP2016.Repository.Utils
{
    public class CustomizedComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> equalsFunc;
        private readonly Func<T, int> getHashCodeFunc;

        public CustomizedComparer(Func<T, T, bool> equalsFunc, Func<T, int> getHashCodeFunc)
        {
            this.equalsFunc = equalsFunc;
            this.getHashCodeFunc = getHashCodeFunc;
        }

        public bool Equals(T x, T y)
        {
            return equalsFunc(x, y);
        }

        public int GetHashCode(T obj)
        {
            return getHashCodeFunc(obj);
        }
    }
}
