using System;

namespace FastMobile.FXamarin.Core
{
    public interface IFNetwork
    {
        event EventHandler ConnectNetworkClicked;

        bool HasNetwork { get; }
    }
}