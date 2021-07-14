using System;

namespace FastMobile.FXamarin.Core
{
    public interface IFSpinWheelControl
    {
        event EventHandler<FSpinWheelEventArgs> SpinClicked;
    }
}