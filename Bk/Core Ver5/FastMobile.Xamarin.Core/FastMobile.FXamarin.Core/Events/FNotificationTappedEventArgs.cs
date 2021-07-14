using System;

namespace FastMobile.FXamarin.Core
{
    public delegate void FNotificationTappedEventHandler(FNotificationTappedEventArgs e);

    public class FNotificationTappedEventArgs : EventArgs
    {
        public string Data { get; }

        public FNotificationTappedEventArgs(string data)
        {
            Data = data;
        }
    }
}