using System;

namespace FastMobile.FXamarin.Core
{
    public class FSelectedChangeEventArgs : EventArgs
    {
        public FSelectedChangeEventArgs()
        {
        }

        public FSelectedChangeEventArgs(object value)
        {
            Value = value;
        }

        public object Value { get; set; }
    }
}