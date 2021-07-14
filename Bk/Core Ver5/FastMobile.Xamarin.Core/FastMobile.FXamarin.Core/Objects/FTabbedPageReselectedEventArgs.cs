using System;

namespace FastMobile.FXamarin.Core
{
    public class FTabbedPageReselectedEventArgs : EventArgs
    {
        public FPage Current { get; }

        public FTabbedPageReselectedEventArgs(FPage page)
        {
            Current = page;
        }
    }
}