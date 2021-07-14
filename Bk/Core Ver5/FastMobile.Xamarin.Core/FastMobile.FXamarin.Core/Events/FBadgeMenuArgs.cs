using System;

namespace FastMobile.FXamarin.Core
{
    public class FBadgeMenuArgs : EventArgs
    {
        public string Badge { get; }
        public string Controller { get; }

        public FBadgeMenuArgs(string badge, string controller)
        {
            Badge = badge;
            Controller = controller;
        }
    }
}