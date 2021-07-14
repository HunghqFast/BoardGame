using System;

namespace FastMobile.FXamarin.Core
{
    public interface IFDataHandler
    {
        event EventHandler<IFDataEvent> ItemTapped;

        void OnItemTapped(object sender, IFDataEvent e);
    }
}