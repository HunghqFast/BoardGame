using System;

namespace FastMobile.FXamarin.Core
{
    public class FObjectPropertyArgs<T> : EventArgs
    {
        public T Value { get; }

        public FObjectPropertyArgs(T val)
        {
            Value = val;
        }
    }
}