using System;

namespace FastMobile.FXamarin.Core
{
    public interface IFBadge
    {
        event EventHandler<FBadgeMenuArgs> ReceivedBadgeForMenu;

        string BadgeValue { get; set; }

        void Clear();

        void OnReceived(string badge, string controller);
    }
}