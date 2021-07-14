using System;

namespace FastMobile.FXamarin.Core
{
    public class FChangeValueArgs : EventArgs
    {
        public object Value { get; }

        public FChangeValueArgs(object val)
        {
            Value = val;
        }
    }
}