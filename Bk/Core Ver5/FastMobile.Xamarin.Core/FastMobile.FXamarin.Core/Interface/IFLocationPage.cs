using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public interface IFLocationPage
    {
        ObservableCollection<FPlace> Places { get; set; }
        ToolbarItem Refresh { get; }
        IList<ToolbarItem> ToolbarItems { get; }
        IFLocation Location { get; set; }
        IFLocationControl Control { get; set; }

        bool IsBusy { get; set; }
        string Title { get; set; }

        void OpenList();

        void CloseList();

        event EventHandler<FObjectPropertyArgs<FPlace>> ItemClicked;
    }
}