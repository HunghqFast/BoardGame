using System;

namespace FastMobile.FXamarin.Core
{
    public interface IFSearch
    {
        string SearchText { get; set; }

        event EventHandler<FSearchEventArgs> SearchBarTextChanged;

        event EventHandler<FSearchEventArgs> SearchBarTextSubmit;
    }
}