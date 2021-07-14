using System;

namespace FastMobile.FXamarin.Core
{
    public class FSearchEventArgs : EventArgs
    {
        public FSearchEventArgs()
        {
        }

        public FSearchEventArgs(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}