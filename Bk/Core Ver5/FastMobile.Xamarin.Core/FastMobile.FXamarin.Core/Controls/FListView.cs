using Syncfusion.ListView.XForms;
using System;
using Xamarin.Forms.Internals;

namespace FastMobile.FXamarin.Core
{
    [Preserve(AllMembers = true)]
    public class FListView : SfListView
    {
        public event EventHandler<FSelectedChangeEventArgs> SelectedChanging;

        public event EventHandler<FSelectedChangeEventArgs> SelectedChanged;

        public FListView() : base()
        {
            SelectionMode = SelectionMode.SingleDeselect;
            SelectionChanged += Changed;
            SelectionChanging += Changing;
        }

        private void Changing(object sender, ItemSelectionChangingEventArgs e)
        {
            OnSelectedChanging(this, new FSelectedChangeEventArgs(SelectedItem));
        }

        private void Changed(object sender, ItemSelectionChangedEventArgs e)
        {
            OnSelectedChanged(this, new FSelectedChangeEventArgs(SelectedItem));
        }

        protected virtual void OnSelectedChanging(object sender, FSelectedChangeEventArgs e)
        {
            SelectedChanging?.Invoke(this, new FSelectedChangeEventArgs(SelectedItem));
        }

        protected virtual void OnSelectedChanged(object sender, FSelectedChangeEventArgs e)
        {
            SelectedChanged?.Invoke(this, new FSelectedChangeEventArgs(SelectedItem));
        }
    }
}