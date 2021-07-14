using System;

namespace FastMobile.FXamarin.Core
{
    public class FDataEventArgs : EventArgs, IFDataEvent
    {
        public object ItemData { get; }

        public FDataEventArgs(object data)
        {
            ItemData = data;
        }
    }
}